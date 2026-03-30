using MediatR;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Application.Schedules.Results;

namespace PlanIt.Application.Schedules.Queries.GetSchedulesByDate;

public class GetSchedulesByDateQueryHandler(
    IScheduleRepository scheduleRepository
    ) : IRequestHandler<GetSchedulesByDateQuery, List<ScheduleResult>>
{
    public async Task<List<ScheduleResult>> Handle(GetSchedulesByDateQuery request, CancellationToken cancellationToken)
    {
        var offset = TimeSpan.FromMinutes(request.UtcOffsetMinutes);
        var startUtc = DateTime.SpecifyKind(request.Date.ToDateTime(TimeOnly.MinValue) - offset, DateTimeKind.Utc);
        var endUtc = DateTime.SpecifyKind(request.Date.ToDateTime(TimeOnly.MaxValue) - offset, DateTimeKind.Utc);

        var schedules = await scheduleRepository.GetByDateRange(startUtc, endUtc);

        return schedules.Select(s => new ScheduleResult(
            s.Schedule.Id,
            s.Schedule.Name,
            s.Schedule.Description,
            s.Schedule.Location,
            s.Schedule.StartTime,
            s.Schedule.EndTime,
            s.AttractionCount
        )).ToList();
    }
}