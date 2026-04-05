using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PlanIt.Application.Common.Interfaces.Persistence;

namespace PlanIt.Infrastructure.Realtime;

[Authorize]
public class AdminHub(
    IUserRepository userRepository,
    IScheduleRepository scheduleRepository,
    IAttractionRepository attractionRepository,
    IRegistrantRepository registrantRepository) : Hub
{
    public async Task<object> JoinDashboard()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "admin");

        var userCount = await userRepository.CountAsync();
        var scheduleCount = await scheduleRepository.CountAsync();
        var attractionCount = await attractionRepository.CountAsync();
        var registrantCount = await registrantRepository.CountAsync();

        return new { userCount, scheduleCount, attractionCount, registrantCount };
    }

    public Task LeaveDashboard() =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, "admin");
}
