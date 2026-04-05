using System.Text.Json;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Application.Registrants.Results;
using PlanIt.Domain.Entities;
using StackExchange.Redis;

namespace PlanIt.Infrastructure.CachedPersistence;

public class CachedRegistrantRepository(
    IRegistrantRepository inner,
    IDatabase cache
    ) : IRegistrantRepository
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan PlansTtl = TimeSpan.FromSeconds(30);

    private static string RemainingCapacityKey(Guid attractionId) => $"attraction:remaining:{attractionId}";
    private static string UserRegistrationsKey(Guid userId) => $"user:registrations:{userId}";
    private static string UserPlansKey(Guid userId) => $"user:plans:{userId}";
    private const string CountKey = "registrant:count";

    public async Task<Registrant> AddAsync(Registrant registrant)
    {
        if (await cache.KeyExistsAsync(CountKey))
            await cache.StringIncrementAsync(CountKey);
        try
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

            await cache.KeyDeleteAsync(UserPlansKey(registrant.UserId));

            return result;
        }
        catch
        {
            if (await cache.KeyExistsAsync(CountKey))
                await cache.StringDecrementAsync(CountKey);
            throw;
        }
    }

    public async Task RemoveAsync(Guid userId, Guid attractionId)
    {
        if (await cache.KeyExistsAsync(CountKey))
            await cache.StringDecrementAsync(CountKey);
        try
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

            await cache.KeyDeleteAsync(UserPlansKey(userId));
        }
        catch
        {
            if (await cache.KeyExistsAsync(CountKey))
                await cache.StringIncrementAsync(CountKey);
            throw;
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

    public async Task<List<MyAttractionResult>> GetUserAttractionsWithDetails(Guid userId)
    {
        var key = UserPlansKey(userId);
        var cached = await cache.StringGetAsync(key);

        if (cached.HasValue)
        {
            var deserialized = JsonSerializer.Deserialize<List<MyAttractionResult>>(cached!);
            if (deserialized is not null)
            {
                await cache.KeyExpireAsync(key, PlansTtl);
                return deserialized;
            }
        }

        var result = await inner.GetUserAttractionsWithDetails(userId);
        await cache.StringSetAsync(key, JsonSerializer.Serialize(result), PlansTtl);
        return result;
    }

    public async Task<int> CountAsync()
    {
        var cached = await cache.StringGetAsync(CountKey);
        if (cached.HasValue && int.TryParse(cached, out var count))
        {
            await cache.KeyExpireAsync(CountKey, Ttl);
            return count;
        }

        var result = await inner.CountAsync();
        await cache.StringSetAsync(CountKey, result, Ttl);
        return result;
    }
}
