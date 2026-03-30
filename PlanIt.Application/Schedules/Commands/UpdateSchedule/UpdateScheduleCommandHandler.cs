using MediatR;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Application.Schedules.Results;

namespace PlanIt.Application.Schedules.Commands.UpdateSchedule;

public class UpdateScheduleCommandHandler(
    IScheduleRepository scheduleRepository
    ) : IRequestHandler<UpdateScheduleCommand, ScheduleResult>
{
    public async Task<ScheduleResult> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await scheduleRepository.GetByIdForUpdate(request.ScheduleId);

        schedule.Name = request.Name;
        schedule.Description = request.Description;
        schedule.Location = request.Location;

        var updated = await scheduleRepository.Update(schedule);

        return new ScheduleResult(
            updated.Id,
            updated.Name,
            updated.Description,
            updated.Location,
            updated.StartTime,
            updated.EndTime,
            updated.Attractions.Count
        );
    }
}
