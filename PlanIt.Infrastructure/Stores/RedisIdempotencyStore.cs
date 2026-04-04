using PlanIt.Application.Common.Interfaces.Stores;
using StackExchange.Redis;

namespace PlanIt.Infrastructure.Stores;

public class RedisIdempotencyStore(IDatabase cache) : IIdempotencyStore<string>
{
    public async Task<string?> GetAsync(string key)
    {
        var value = await cache.StringGetAsync(key);
        return value.IsNullOrEmpty ? null : value.ToString();
    }

    public async Task SaveAsync(string key, string record, TimeSpan expiry)
    {
        await cache.StringSetAsync(key, record, expiry, When.NotExists);
    }
}