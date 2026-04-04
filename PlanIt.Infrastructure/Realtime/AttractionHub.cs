using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PlanIt.Application.Common.Interfaces.Persistence;

namespace PlanIt.Infrastructure.Realtime;

[Authorize]
public class AttractionHub(IAttractionRepository attractionRepository) : Hub
{
    public async Task<IEnumerable<object>> JoinSchedule(string scheduleId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"schedule:{scheduleId}");

        var attractions = await attractionRepository.GetByScheduleId(Guid.Parse(scheduleId));

        var capacities = await Task.WhenAll(attractions.Select(async a => new
        {
            attractionId = a.Id,
            remaining = await attractionRepository.GetRemainingCapacity(a.Id)
        } as object));

        return capacities;
    }

    public Task LeaveSchedule(string scheduleId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, $"schedule:{scheduleId}");
}
