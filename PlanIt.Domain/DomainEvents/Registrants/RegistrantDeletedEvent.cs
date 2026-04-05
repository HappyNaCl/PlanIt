using PlanIt.Domain.Common.DomainEvents;
using PlanIt.Domain.Entities;

namespace PlanIt.Domain.DomainEvents.Registrants;

public record RegistrantDeletedEvent(Registrant Registrant) : IDomainEvent
{
    public DateTime OccuredOn { get; } = DateTime.UtcNow;
}