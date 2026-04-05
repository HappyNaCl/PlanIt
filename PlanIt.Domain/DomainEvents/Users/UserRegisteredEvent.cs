using PlanIt.Domain.Common.DomainEvents;
using PlanIt.Domain.Entities;

namespace PlanIt.Domain.DomainEvents.Users;

public record UserRegisteredEvent(User user) : IDomainEvent
{
    public DateTime OccuredOn { get; } = DateTime.UtcNow;
}