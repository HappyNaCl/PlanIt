using Microsoft.AspNetCore.SignalR;
using PlanIt.Application.Common.Interfaces.Realtime;

namespace PlanIt.Infrastructure.Realtime;

public class AdminNotifier(IHubContext<AdminHub> hubContext) : IAdminNotifier
{
    public Task BroadcastStatsUpdate(int userCount, int scheduleCount, int attractionCount, int registrantCount) =>
        hubContext.Clients
            .Group("admin")
            .SendAsync("StatsUpdated", new { userCount, scheduleCount, attractionCount, registrantCount });
}
