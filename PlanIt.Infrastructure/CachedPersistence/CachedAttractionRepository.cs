using System.Text.Json;
using System.Text.Json.Serialization;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Domain.Entities;
using StackExchange.Redis;

namespace PlanIt.Infrastructure.CachedPersistence;

public class CachedAttractionRepository(
    IAttractionRepository inner,
    IDatabase cache
    ) : IAttractionRepository
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);

    private static readonly JsonSerializerOptions Json = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    private static string IdKey(Guid attractionId) => $"attraction:id:{attractionId}";
    private static string ScheduleKey(Guid scheduleId) => $"schedule:attraction:{scheduleId}";
    private static string RemainingCapacityKey(Guid attractionId) => $"attraction:remaining:{attractionId}";

    public async Task<Attraction> Create(Attraction attraction)
    {
        var result = await inner.Create(attraction);
        await cache.KeyDeleteAsync(ScheduleKey(attraction.ScheduleId));
        return result;
    }

    public async Task<Attraction> Delete(Guid attractionId)
    {
        var result = await inner.Delete(attractionId);
        await cache.KeyDeleteAsync(IdKey(attractionId));
        await cache.KeyDeleteAsync(RemainingCapacityKey(attractionId));
        await cache.KeyDeleteAsync(ScheduleKey(result.ScheduleId));
        return result;
    }

    public async Task<Attraction> Update(Attraction attraction)
    {
        var result = await inner.Update(attraction);
        var remaining = await inner.GetRemainingCapacity(result.Id);

        await cache.KeyDeleteAsync(IdKey(result.Id));
        await cache.StringSetAsync(RemainingCapacityKey(result.Id), remaining, Ttl);
        return result;
    }

    public async Task<Attraction> GetById(Guid attractionId)
    {
        var key = IdKey(attractionId);
        var json = await cache.StringGetAsync(key);

        if (!json.IsNullOrEmpty)
        {
            var cached = JsonSerializer.Deserialize<Attraction>(json!, Json);
            if (cached is not null)
            {
                await cache.KeyExpireAsync(key, Ttl);
                return cached;
            }
        }

        var result = await inner.GetById(attractionId);
        await cache.StringSetAsync(key, JsonSerializer.Serialize(result, Json), Ttl);
        return result;
    }

    public async Task<List<Attraction>> GetByScheduleId(Guid scheduleId)
    {
        var cachedIds = await cache.SetMembersAsync(ScheduleKey(scheduleId));

        if (cachedIds.Length > 0)
        {
            var attractions = new List<Attraction>();
            var cacheHit = true;

            // If all id from cachedIds have cached data, it will break the loop and return
            // If one miss, fetch from repo and populate cache
            foreach (var idValue in cachedIds)
            {
                var json = await cache.StringGetAsync(IdKey(Guid.Parse(idValue!)));
                if (json.IsNullOrEmpty)
                {
                    cacheHit = false;
                    break;
                }

                var attraction = JsonSerializer.Deserialize<Attraction>(json!, Json);
                if (attraction is null)
                {
                    cacheHit = false;
                    break;
                }

                attractions.Add(attraction);
            }

            if (cacheHit)
                return attractions;
        }

        var result = await inner.GetByScheduleId(scheduleId);
        await PopulateCache(scheduleId, result);
        return result;
    }

    public Task<Attraction> GetByIdForUpdate(Guid attractionId)
    {
        return inner.GetByIdForUpdate(attractionId);
    }

    public async Task<int> GetRemainingCapacity(Guid attractionId)
    {
        var key = RemainingCapacityKey(attractionId);
        var cached = await cache.StringGetAsync(key);

        if (cached.HasValue && int.TryParse(cached, out var remaining))
        {
            await cache.KeyExpireAsync(key, Ttl);
            return remaining;
        }

        var result = await inner.GetRemainingCapacity(attractionId);
        await cache.StringSetAsync(key, result, Ttl);
        return result;
    }

    private async Task PopulateCache(Guid scheduleId, List<Attraction> attractions)
    {
        foreach (var attraction in attractions)
        {
            var remainingCapacity = attraction.Capacity - attraction.Registrants.Count;
            attraction.Registrants.Clear();

            await cache.StringSetAsync(
                IdKey(attraction.Id),
                JsonSerializer.Serialize(attraction, Json),
                Ttl);

            await cache.SetAddAsync(ScheduleKey(scheduleId), attraction.Id.ToString());

            await cache.StringSetAsync(
                RemainingCapacityKey(attraction.Id),
                remainingCapacity,
                Ttl);
        }

        await cache.KeyExpireAsync(ScheduleKey(scheduleId), Ttl);
    }
}