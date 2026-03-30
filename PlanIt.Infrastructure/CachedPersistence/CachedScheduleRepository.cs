using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Domain.Entities;

namespace PlanIt.Infrastructure.CachedPersistence;

public class CachedScheduleRepository(
    IScheduleRepository inner,
    IDistributedCache cache) : IScheduleRepository
{
    private readonly JsonSerializerOptions _json = new();

    private readonly DistributedCacheEntryOptions _options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
    };    
    
    private static string IdKey(Guid id) => $"schedule:id:{id}";
    private static string DateKey(DateOnly date) => $"schedules:date:{date:yyyy-MM-dd}";
    
    public async Task<Schedule> Create(Schedule schedule)
    {
        var savedSchedule = await inner.Create(schedule);

        await cache.SetStringAsync(IdKey(savedSchedule.Id), JsonSerializer.Serialize(savedSchedule, _json), _options);
        await cache.RemoveAsync(DateKey(DateOnly.FromDateTime(savedSchedule.StartTime)));

        return savedSchedule;
    }

    public async Task<Schedule> Update(Schedule schedule)
    {
        var updatedSchedule = await inner.Update(schedule);

        await cache.SetStringAsync(IdKey(updatedSchedule.Id), JsonSerializer.Serialize(updatedSchedule, _json), _options);
        await cache.RemoveAsync(DateKey(DateOnly.FromDateTime(updatedSchedule.StartTime)));

        return updatedSchedule;
    }

    public async Task<Schedule> Delete(Guid scheduleId)
    {
        var deletedSchedule = await inner.Delete(scheduleId);

        await cache.RemoveAsync(IdKey(scheduleId));
        await cache.RemoveAsync(DateKey(DateOnly.FromDateTime(deletedSchedule.StartTime)));

        return deletedSchedule;
    }

    public async Task<Schedule> GetById(Guid scheduleId)
    {
        var cached = await cache.GetStringAsync(IdKey(scheduleId));

        if (cached != null)
        {
            var deserialized = JsonSerializer.Deserialize<Schedule>(cached, _json);
            if (deserialized is not null)
                return deserialized;
        }

        return await inner.GetById(scheduleId);
    }

    public async Task<Schedule> GetByIdForUpdate(Guid scheduleId)
    {
        return await inner.GetByIdForUpdate(scheduleId);
    }

    public async Task<List<Schedule>> GetByDate(DateOnly date)
    {
        var dateKey = DateKey(date);
        var cachedIds = await cache.GetStringAsync(dateKey);

        if (cachedIds != null)
        {
            var ids = JsonSerializer.Deserialize<List<Guid>>(cachedIds, _json);
            if (ids is not null)
                return await GetByIds(ids);
        }

        var dbSchedules = await inner.GetByDate(date);

        foreach (var schedule in dbSchedules)
            await cache.SetStringAsync(IdKey(schedule.Id), JsonSerializer.Serialize(schedule, _json), _options);

        var idList = dbSchedules.ConvertAll(s => s.Id);
        await cache.SetStringAsync(dateKey, JsonSerializer.Serialize<List<Guid>>(idList, _json), _options);

        return dbSchedules;
    }

    public async Task<List<(Schedule Schedule, int AttractionCount)>> GetByDateRange(DateTime startUtc, DateTime endUtc)
    {
        return await inner.GetByDateRange(startUtc, endUtc);
    }

    public async Task<List<Schedule>> GetByIds(List<Guid> ids)
    {
        var cached = new Dictionary<Guid, Schedule>();
        var missing = new List<Guid>();

        foreach (var id in ids)
        {
            var raw = await cache.GetStringAsync(IdKey(id));
            if (raw != null)
            {
                var schedule = JsonSerializer.Deserialize<Schedule>(raw, _json);
                if (schedule is not null)
                {
                    cached[id] = schedule;
                    continue;
                }
            }
            missing.Add(id);
        }

        if (missing.Count > 0)
        {
            var fetched = await inner.GetByIds(missing);
            foreach (var schedule in fetched)
            {
                await cache.SetStringAsync(IdKey(schedule.Id), JsonSerializer.Serialize(schedule, _json), _options);
                cached[schedule.Id] = schedule;
            }
        }

        return ids.Where(cached.ContainsKey).Select(id => cached[id]).ToList();
    }
}