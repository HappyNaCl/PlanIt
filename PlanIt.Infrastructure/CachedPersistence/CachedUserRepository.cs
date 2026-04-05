using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Domain.Entities;

namespace PlanIt.Infrastructure.CachedPersistence;

public class CachedUserRepository(IUserRepository inner, IDistributedCache cache) : IUserRepository
{
    private readonly JsonSerializerOptions _json = new();

    private readonly DistributedCacheEntryOptions _options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
        SlidingExpiration = TimeSpan.FromMinutes(10),
    };    

    private static string RedisKey(Guid id) => $"user:id:{id}";
    private const string CountKey = "user:count";
    
    public async Task<User> Create(User user)
    {
        await AdjustCountAsync(1);
        try
        {
            var savedUser = await inner.Create(user);
            await cache.SetStringAsync(RedisKey(savedUser.Id), JsonSerializer.Serialize(savedUser, _json), _options);
            return savedUser;
        }
        catch
        {
            await AdjustCountAsync(-1);
            throw;
        }
    }

    public async Task<User> GetById(Guid id)
    {
        var cached = await cache.GetStringAsync(RedisKey(id));

        if (cached == null) return await inner.GetById(id);
        
        var deserialized = JsonSerializer.Deserialize<User>(cached, _json);
        if (deserialized is not null)
            return deserialized;

        return await inner.GetById(id);
    }
    
    public async Task<User?> GetByUsernameDefault(string username)
    {
        return await inner.GetByUsernameDefault(username);
    }

    public async Task<int> CountAsync()
    {
        var cached = await cache.GetStringAsync(CountKey);
        if (cached != null && int.TryParse(cached, out var count))
            return count;

        var result = await inner.CountAsync();
        await cache.SetStringAsync(CountKey, result.ToString(), _options);
        return result;
    }

    private async Task AdjustCountAsync(int delta)
    {
        var cached = await cache.GetStringAsync(CountKey);
        if (cached != null && int.TryParse(cached, out var count))
            await cache.SetStringAsync(CountKey, (count + delta).ToString(), _options);
    }
}