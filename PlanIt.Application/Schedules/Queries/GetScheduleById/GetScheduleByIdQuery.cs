using MediatR;
using PlanIt.Application.Schedules.Results;

namespace PlanIt.Application.Schedules.Queries.GetScheduleById;

public record GetScheduleByIdQuery(
    Guid ScheduleId) : IRequest<DetailedScheduleResult>;