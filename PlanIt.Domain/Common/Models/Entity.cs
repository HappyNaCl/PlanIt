using PlanIt.Domain.Common.Interfaces;

namespace PlanIt.Domain.Common.Models;

public abstract class Entity<TId>(TId id) : IEquatable<Entity<TId>>, ISoftDeletable
    where TId : notnull
{
    public TId Id { get; init; } = id;
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; private set; }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> other && Id.Equals(other.Id);
    }

    public static bool operator ==(Entity<TId> left, Entity<TId> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TId> left, Entity<TId> right)
    {
        return !Equals(left, right);
    }
    
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(Entity<TId>? other)
    {
        return Equals((object?) other);
    }
}