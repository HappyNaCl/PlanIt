using MediatR;
using PlanIt.Application.Common.Interfaces.FileUploader;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Application.Schedules.Results;

namespace PlanIt.Application.Schedules.Queries.GetScheduleById;

public class GetScheduleByIdQueryHandler(
    IScheduleRepository scheduleRepository,
    IAttractionRepository attractionRepository,
    IFileUploader fileUploader
    ) : IRequestHandler<GetScheduleByIdQuery, DetailedScheduleResult>
{
    public async Task<DetailedScheduleResult> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        var schedule = await scheduleRepository.GetById(request.ScheduleId);
        var attractions = await attractionRepository.GetByScheduleId(schedule.Id);
        
        var attractionDtos = new List<AttractionResultDto>();
        foreach (var a in attractions)
        {
            var remaining = await attractionRepository.GetRemainingCapacity(a.Id);
            attractionDtos.Add(new AttractionResultDto(
                a.Id,
                a.Name,
                a.Description,
                $"{fileUploader.GetEndpoint()}/{a.ImageKey}",
                a.Capacity,
                remaining
            ));
        }

        return new DetailedScheduleResult(
            schedule.Id,
            schedule.Name,
            schedule.Description,
            schedule.Location,
            schedule.StartTime,
            schedule.EndTime,
            attractionDtos
        );
    }
}