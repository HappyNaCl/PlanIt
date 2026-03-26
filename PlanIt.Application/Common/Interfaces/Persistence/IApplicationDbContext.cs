using Microsoft.EntityFrameworkCore;
using PlanIt.Domain.Entities;

namespace PlanIt.Application.Common.Interfaces.Persistence;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Schedule> Schedules { get; }
    DbSet<Attraction> Attractions { get; }
    DbSet<Registrant> Registrants { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}