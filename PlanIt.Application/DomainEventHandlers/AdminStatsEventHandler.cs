using MediatR;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Application.Common.Interfaces.Realtime;
using PlanIt.Domain.DomainEvents.Attractions;
using PlanIt.Domain.DomainEvents.Registrants;
using PlanIt.Domain.DomainEvents.Schedules;
using PlanIt.Domain.DomainEvents.Users;

namespace PlanIt.Application.DomainEventHandlers;

public class AdminStatsEventHandler(
    IUserRepository userRepository,
    IScheduleRepository scheduleRepository,
    IAttractionRepository attractionRepository,
    IRegistrantRepository registrantRepository,
    IAdminNotifier adminNotifier) :
    INotificationHandler<UserRegisteredEvent>,
    INotificationHandler<ScheduleCreatedEvent>,
    INotificationHandler<ScheduleDeletedEvent>,
    INotificationHandler<AttractionCreatedEvent>,
    INotificationHandler<AttractionDeletedEvent>,
    INotificationHandler<RegistrantCreatedEvent>,
    INotificationHandler<RegistrantDeletedEvent>
{
    public Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken) => BroadcastStats(cancellationToken);
    public Task Handle(ScheduleCreatedEvent notification, CancellationToken cancellationToken) => BroadcastStats(cancellationToken);
    public Task Handle(ScheduleDeletedEvent notification, CancellationToken cancellationToken) => BroadcastStats(cancellationToken);
    public Task Handle(AttractionCreatedEvent notification, CancellationToken cancellationToken) => BroadcastStats(cancellationToken);
    public Task Handle(AttractionDeletedEvent notification, CancellationToken cancellationToken) => BroadcastStats(cancellationToken);
    public Task Handle(RegistrantCreatedEvent notification, CancellationToken cancellationToken) => BroadcastStats(cancellationToken);
    public Task Handle(RegistrantDeletedEvent notification, CancellationToken cancellationToken) => BroadcastStats(cancellationToken);

    private async Task BroadcastStats(CancellationToken cancellationToken = default)
    {
        var userCount = await userRepository.CountAsync();
        var scheduleCount = await scheduleRepository.CountAsync();
        var attractionCount = await attractionRepository.CountAsync();
        var registrantCount = await registrantRepository.CountAsync();
        
        Console.WriteLine("[AdminStatsEventHandler] Updating...! ");

        await adminNotifier.BroadcastStatsUpdate(userCount, scheduleCount, attractionCount, registrantCount);
    }
}
