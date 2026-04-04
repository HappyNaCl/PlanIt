namespace PlanIt.Application.Common.Interfaces.Locking;

public interface IDistributedLock
{
    Task<string?> AcquireAsync(string resource, TimeSpan expiry);
    Task ReleaseAsync(string resource, string token);
}
