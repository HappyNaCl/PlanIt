using PlanIt.Domain.Common.DomainEvents;
using PlanIt.Domain.Entities;

namespace PlanIt.Domain.DomainEvents.Attractions;

public record AttractionCreatedEvent(Attraction Attraction) : IDomainEvent
{
    public DateTime OccuredOn { get; } = DateTime.UtcNow;
}