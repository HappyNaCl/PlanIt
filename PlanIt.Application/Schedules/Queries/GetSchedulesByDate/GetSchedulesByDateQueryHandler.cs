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
        var schedules = await scheduleRepository.GetByDate(request.Date);

        return schedules.Select(s => new ScheduleResult(
            s.Id,
            s.Name,
            s.Description,
            s.Location,
            s.StartTime,
            s.EndTime,
            s.Attractions.Count
        )).ToList();
    }
}