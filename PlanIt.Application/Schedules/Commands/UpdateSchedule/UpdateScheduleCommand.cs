using MediatR;
using PlanIt.Application.Schedules.Results;

namespace PlanIt.Application.Schedules.Commands.UpdateSchedule;

public record UpdateScheduleCommand(
    Guid ScheduleId,
    string Name,
    string Description,
    string Location) : IRequest<ScheduleResult>;
