using MediatR;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Application.Schedules.Results;
using PlanIt.Domain.Entities;

namespace PlanIt.Application.Schedules.Commands.CreateSchedule;

public class CreateScheduleCommandHandler(
    IScheduleRepository scheduleRepository
    ) : IRequestHandler<CreateScheduleCommand, ScheduleResult>
{
    public async Task<ScheduleResult> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        var newSchedule = Schedule.Create(request.Name, request.Description, request.Location, request.StartTime.ToUniversalTime(), request.EndTime.ToUniversalTime());

        var savedSchedule = await scheduleRepository.Create(newSchedule);

        return new ScheduleResult
        (
            savedSchedule.Id,
            savedSchedule.Name,
            savedSchedule.Description,
            savedSchedule.Location,
            savedSchedule.StartTime,
            savedSchedule.EndTime,
            0
        );
    }
}