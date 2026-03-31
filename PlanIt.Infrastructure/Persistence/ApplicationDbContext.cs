using System.Linq.Expressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Domain.Common.Interfaces;
using PlanIt.Domain.Common.Models;
using PlanIt.Domain.Entities;

namespace PlanIt.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator? mediator = null) :
    DbContext(options), IApplicationDbContext
{
    private readonly IMediator? _mediator = mediator;
    
    public DbSet<User> Users => Set<User>();
    
    public DbSet<Schedule> Schedules => Set<Schedule>();
    
    public DbSet<Attraction> Attractions => Set<Attraction>();
    
    public DbSet<Registrant> Registrants =>  Set<Registrant>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(
            entity => {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasConversion<string>();
                entity.HasMany(e => e.RegisteredAttractions)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        );

        modelBuilder.Entity<Schedule>(
            entity => {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(150);
                entity.Property(e => e.Location).HasMaxLength(100);
                entity.Property(e => e.StartTime).IsRequired().HasColumnType("timestamptz");
                entity.Property(e => e.EndTime).IsRequired().HasColumnType("timestamptz");
                entity.HasMany(e => e.Attractions)
                      .WithOne(a => a.Schedule)
                      .HasForeignKey(a => a.ScheduleId)
                      .OnDelete(DeleteBehavior.Cascade);
            }
        );

        modelBuilder.Entity<Attraction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ScheduleId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).IsRequired(false).HasMaxLength(150);
            entity.Property(e => e.ImageKey).HasMaxLength(75);
            entity.Property(e => e.Capacity).IsRequired();
            entity.HasMany(e => e.Registrants)
                .WithOne(r => r.Attraction)
                .HasForeignKey(r => r.AttractionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Registrant>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.AttractionId });
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.AttractionId);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Token);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(50);
            entity.Property(e => e.IsUsed).IsRequired();
            entity.Property(e => e.ExpiresAt).IsRequired();
        });
        
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(BuildSoftDeleteFilter(entityType.ClrType));
            }
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ConvertHardDeletesToSoftDeletes();
        UpdateTimestamps();
        
        // var domainEntities = ChangeTracker
        //     .Entries<Entity<Guid>>()
        //     .Where(x => x.Entity.DomainEvents.Any())
        //     .ToList();
        //
        // var domainEvents = domainEntities
        //     .SelectMany(x => x.Entity.DomainEvents)
        //     .ToList();
        //
        // var result = await base.SaveChangesAsync(cancellationToken);
        //
        // foreach (var domainEvent in domainEvents)
        //     await _mediator.Publish(domainEvent, cancellationToken);
        //
        // domainEntities.ForEach(e => e.Entity.ClearDomainEvents());
        //
        // return result;
        
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<Entity<Guid>>();
        var utcNow = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.UpdatedAt = utcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utcNow;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    private void ConvertHardDeletesToSoftDeletes()
    {
        var deletedEntries = ChangeTracker
            .Entries<ISoftDeletable>()
            .Where(e => e.State == EntityState.Deleted);

        foreach (var entry in deletedEntries)
        {
            entry.State = EntityState.Modified;
            if (entry.Entity is Entity<Guid> entity)
            {
                entity.Delete();
            }
        }
    }

    private static LambdaExpression BuildSoftDeleteFilter(Type type)
    {
        var param = Expression.Parameter(type, "e");
        var body = Expression.Equal(
            Expression.Property(param, nameof(ISoftDeletable.IsDeleted)),
            Expression.Constant(false)
        );
        return Expression.Lambda(body, param);
    }

}