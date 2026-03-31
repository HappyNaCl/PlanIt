using MediatR;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Application.Schedules.Results;

namespace PlanIt.Application.Schedules.Queries.GetScheduleById;

public class GetScheduleByIdQueryHandler(
    IScheduleRepository scheduleRepository,
    IAttractionRepository attractionRepository
    ) : IRequestHandler<GetScheduleByIdQuery, DetailedScheduleResult>
{
    public async Task<DetailedScheduleResult> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        var schedule = await scheduleRepository.GetById(request.ScheduleId);
        var attractions = await attractionRepository.GetByScheduleId(schedule.Id);
        
        return new DetailedScheduleResult(
            schedule.Id,
            schedule.Name,
            schedule.Description,
            schedule.Location,
            schedule.StartTime,
            schedule.EndTime,
            attractions.Select(a => new AttractionResultDto(
                    a.Id,
                    a.Name,
                    a.Description,
                    a.ImageKey,
                    a.Capacity,
                    a.Capacity - a.Registrants.Count
                )).ToList()
        );
    }
}