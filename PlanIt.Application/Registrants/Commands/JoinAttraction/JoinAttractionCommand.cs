using MediatR;
using PlanIt.Application.Common.Interfaces.Idempotency;

namespace PlanIt.Application.Registrants.Commands.JoinAttraction;

public record JoinAttractionCommand(
    Guid AttractionId,
    Guid ScheduleId,
    Guid UserId,
    string IdempotencyKey) : IRequest, IIdempotencyCommand;
