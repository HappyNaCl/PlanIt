using MediatR;

namespace PlanIt.Application.Registrants.Commands.LeaveAttraction;

public record LeaveAttractionCommand(
    Guid AttractionId,
    Guid ScheduleId,
    Guid UserId) : IRequest;
