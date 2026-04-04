using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Domain.Entities;
using StackExchange.Redis;

namespace PlanIt.Infrastructure.CachedPersistence;

public class CachedRegistrantRepository(
    IRegistrantRepository inner,
    IDatabase cache
    ) : IRegistrantRepository
{
    private static string RemainingCapacityKey(Guid attractionId) => $"attraction:remaining:{attractionId}";

    public async Task<Registrant> AddAsync(Registrant registrant)
    {
        var result = await inner.AddAsync(registrant);
        var key = RemainingCapacityKey(registrant.AttractionId);
        if (await cache.KeyExistsAsync(key))
            await cache.StringDecrementAsync(key);
        return result;
    }

    public async Task RemoveAsync(Guid userId, Guid attractionId)
    {
        await inner.RemoveAsync(userId, attractionId);
        var key = RemainingCapacityKey(attractionId);
        if (await cache.KeyExistsAsync(key))
            await cache.StringIncrementAsync(key);
    }
}