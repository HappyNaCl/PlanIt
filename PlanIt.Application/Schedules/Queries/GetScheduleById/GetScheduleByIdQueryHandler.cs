using MediatR;
using PlanIt.Application.Common.Interfaces.FileUploader;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Application.Schedules.Results;

namespace PlanIt.Application.Schedules.Queries.GetScheduleById;

public class GetScheduleByIdQueryHandler(
    IScheduleRepository scheduleRepository,
    IAttractionRepository attractionRepository,
    IRegistrantRepository registrantRepository,
    IFileUploader fileUploader
    ) : IRequestHandler<GetScheduleByIdQuery, ScheduleResult>
{
    public async Task<ScheduleResult> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        var schedule = await scheduleRepository.GetById(request.ScheduleId);
        
        return new ScheduleResult(
            schedule.Id,
            schedule.Name,
            schedule.Description,
            schedule.Location,
            schedule.StartTime,
            schedule.EndTime,
            schedule.Attractions.Count
        );
    }
}
