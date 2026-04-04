using Microsoft.AspNetCore.SignalR;
using PlanIt.Application.Attractions.Results;
using PlanIt.Application.Common.Interfaces.Realtime;

namespace PlanIt.Infrastructure.Realtime;

public class AttractionNotifier(IHubContext<AttractionHub> hubContext) : IAttractionNotifier
{
    public Task BroadcastCapacityUpdate(Guid scheduleId, Guid attractionId, int remaining) =>
        hubContext.Clients
            .Group($"schedule:{scheduleId}")
            .SendAsync("CapacityUpdated", new { attractionId, remaining });

    public Task SendRegistrationConfirmed(string userId, AttractionResult result) =>
        hubContext.Clients
            .User(userId)
            .SendAsync("RegistrationConfirmed", result);

    public Task SendRegistrationFailed(string userId, string reason) =>
        hubContext.Clients
            .User(userId)
            .SendAsync("RegistrationFailed", new { reason });
}
