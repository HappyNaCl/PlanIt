using PlanIt.Application.Common.Interfaces.Locking;
using StackExchange.Redis;

namespace PlanIt.Infrastructure.Locking;

public class RedisDistributedLock(IDatabase cache) : IDistributedLock
{
    public async Task<string?> AcquireAsync(string resource, TimeSpan expiry)
    {
        var token = Guid.NewGuid().ToString();
        var acquired = await cache.StringSetAsync(resource, token, expiry, When.NotExists);
        return acquired ? token : null;
    }

    public async Task ReleaseAsync(string resource, string token)
    {
        var current = await cache.StringGetAsync(resource);
        if (current == token)
            await cache.KeyDeleteAsync(resource);
    }
}
