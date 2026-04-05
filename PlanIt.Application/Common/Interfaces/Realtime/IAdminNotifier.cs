namespace PlanIt.Application.Common.Interfaces.Realtime;

public interface IAdminNotifier
{
    Task BroadcastStatsUpdate(int userCount, int scheduleCount, int attractionCount, int registrantCount);
}
