using MediatR;

namespace PlanIt.Application.Schedules.Commands.DeleteSchedule;

public record DeleteScheduleCommand(Guid ScheduleId) : IRequest;
