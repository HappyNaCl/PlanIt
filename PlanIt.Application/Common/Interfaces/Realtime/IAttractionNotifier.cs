using PlanIt.Application.Attractions.Results;

namespace PlanIt.Application.Common.Interfaces.Realtime;

public interface IAttractionNotifier
{
    Task BroadcastCapacityUpdate(Guid scheduleId, Guid attractionId, int remaining);
    Task SendRegistrationConfirmed(string userId, AttractionResult result);
    Task SendRegistrationFailed(string userId, string reason);
}
