using MediatR;
using PlanIt.Application.Schedules.Results;

namespace PlanIt.Application.Schedules.Commands.CreateSchedule;

public record CreateScheduleCommand(
    string Name,
    string Description,
    string Location,
    DateTime StartTime,
    DateTime EndTime) : IRequest<ScheduleResult>;