using MediatR;
using PlanIt.Application.Schedules.Results;

namespace PlanIt.Application.Schedules.Queries.GetSchedulesByDate;

public record GetSchedulesByDateQuery(DateOnly Date, int UtcOffsetMinutes) : IRequest<List<ScheduleResult>>;