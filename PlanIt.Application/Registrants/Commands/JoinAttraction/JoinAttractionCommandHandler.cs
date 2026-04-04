using MediatR;
using PlanIt.Application.Common.Interfaces.Messaging;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Application.Registrants.Messages;
using PlanIt.Domain.Common.Exceptions.Registrants;

namespace PlanIt.Application.Registrants.Commands.JoinAttraction;

public class JoinAttractionCommandHandler(
    IAttractionRepository attractionRepository,
    IEventBus eventBus
    ) : IRequestHandler<JoinAttractionCommand>
{
    public async Task Handle(JoinAttractionCommand request, CancellationToken cancellationToken)
    {
        var remaining = await attractionRepository.GetRemainingCapacity(request.AttractionId);
        if (remaining <= 0)
            throw new AttractionFullException(request.AttractionId);

        await eventBus.PublishAsync(
            new JoinAttractionMessage(
                request.AttractionId,
                request.ScheduleId,
                request.UserId,
                request.IdempotencyKey),
            JoinAttractionMessage.Queue);
    }
}
