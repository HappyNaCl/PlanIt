using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Domain.Entities;
using StackExchange.Redis;

namespace PlanIt.Infrastructure.CachedPersistence;

public class CachedRegistrantRepository(
    IRegistrantRepository inner,
    IDatabase cache
    ) : IRegistrantRepository
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);

    private static string RemainingCapacityKey(Guid attractionId) => $"attraction:remaining:{attractionId}";
    private static string UserRegistrationsKey(Guid userId) => $"user:registrations:{userId}";

    public async Task<Registrant> AddAsync(Registrant registrant)
    {
        var result = await inner.AddAsync(registrant);

        var remainingKey = RemainingCapacityKey(registrant.AttractionId);
        if (await cache.KeyExistsAsync(remainingKey))
            await cache.StringDecrementAsync(remainingKey);

        var userKey = UserRegistrationsKey(registrant.UserId);
        if (await cache.KeyExistsAsync(userKey))
        {
            await cache.SetAddAsync(userKey, registrant.AttractionId.ToString());
            await cache.KeyExpireAsync(userKey, Ttl);
        }

        return result;
    }

    public async Task RemoveAsync(Guid userId, Guid attractionId)
    {
        await inner.RemoveAsync(userId, attractionId);

        var remainingKey = RemainingCapacityKey(attractionId);
        if (await cache.KeyExistsAsync(remainingKey))
            await cache.StringIncrementAsync(remainingKey);

        var userKey = UserRegistrationsKey(userId);
        if (await cache.KeyExistsAsync(userKey))
        {
            await cache.SetRemoveAsync(userKey, attractionId.ToString());
            await cache.KeyExpireAsync(userKey, Ttl);
        }
    }

    public async Task<ISet<Guid>> GetRegisteredAttractionIds(Guid userId)
    {
        var key = UserRegistrationsKey(userId);
        var members = await cache.SetMembersAsync(key);

        if (members.Length > 0)
        {
            await cache.KeyExpireAsync(key, Ttl);
            return members.Select(m => Guid.Parse(m!)).ToHashSet();
        }

        var ids = await inner.GetRegisteredAttractionIds(userId);

        if (ids.Count > 0)
        {
            await cache.SetAddAsync(key, ids.Select(id => (RedisValue)id.ToString()).ToArray());
            await cache.KeyExpireAsync(key, Ttl);
        }

        return ids;
    }
}
