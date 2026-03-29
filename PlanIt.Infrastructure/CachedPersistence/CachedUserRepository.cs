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
    
    public async Task<User> Create(User user)
    {
        var savedUser = await inner.Create(user);
        
        var cacheKey = RedisKey(savedUser.Id);
        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(user, _json), _options);
        
        return savedUser;
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
}