using MediatR;

namespace PlanIt.Domain.Common.DomainEvents;


public interface IDomainEvent : INotification
{
    DateTime OccuredOn { get; }
}