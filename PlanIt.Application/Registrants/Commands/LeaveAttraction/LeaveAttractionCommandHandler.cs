using MediatR;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Application.Common.Interfaces.Realtime;

namespace PlanIt.Application.Registrants.Commands.LeaveAttraction;

public class LeaveAttractionCommandHandler(
    IRegistrantRepository registrantRepository,
    IAttractionRepository attractionRepository,
    IAttractionNotifier notifier
    ) : IRequestHandler<LeaveAttractionCommand>
{
    public async Task Handle(LeaveAttractionCommand request, CancellationToken cancellationToken)
    {
        await registrantRepository.RemoveAsync(request.UserId, request.AttractionId);

        var remaining = await attractionRepository.GetRemainingCapacity(request.AttractionId);
        await notifier.BroadcastCapacityUpdate(request.ScheduleId, request.AttractionId, remaining);
    }
}
