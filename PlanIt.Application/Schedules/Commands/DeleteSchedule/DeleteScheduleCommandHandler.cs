using MediatR;
using PlanIt.Application.Common.Interfaces.Persistence;

namespace PlanIt.Application.Schedules.Commands.DeleteSchedule;

public class DeleteScheduleCommandHandler(
    IScheduleRepository scheduleRepository
    ) : IRequestHandler<DeleteScheduleCommand>
{
    public async Task Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
    {
        await scheduleRepository.Delete(request.ScheduleId);
    }
}
