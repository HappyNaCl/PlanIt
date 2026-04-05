using PlanIt.Domain.Common.DomainEvents;
using PlanIt.Domain.Entities;

namespace PlanIt.Domain.DomainEvents.Schedules;

public record ScheduleDeletedEvent(Schedule Schedule) : IDomainEvent
{
    public DateTime OccuredOn { get; } = DateTime.UtcNow;
}