using Microsoft.EntityFrameworkCore;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Domain.Common.Exceptions.Schedules;
using PlanIt.Domain.Entities;

namespace PlanIt.Infrastructure.Persistence;

public class ScheduleRepository(IApplicationDbContext context) : IScheduleRepository
{
    public async Task<Schedule> Create(Schedule schedule)
    {
        context.Schedules.Add(schedule);
        await context.SaveChangesAsync(CancellationToken.None);
        return schedule;
    }

    public async Task<Schedule> Update(Schedule schedule)
    {
        context.Schedules.Update(schedule);
        await context.SaveChangesAsync(CancellationToken.None);
        return schedule;
    }

    public async Task<Schedule> Delete(Guid scheduleId)
    {
        var schedule = await context.Schedules
            .FirstOrDefaultAsync(s => s.Id == scheduleId)
            ?? throw new ScheduleNotFoundException(scheduleId);

        context.Schedules.Remove(schedule);
        await context.SaveChangesAsync(CancellationToken.None);
        return schedule;
    }

    public async Task<Schedule> GetById(Guid scheduleId)
    {
        var schedule = await context.Schedules
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == scheduleId)
            ?? throw new ScheduleNotFoundException(scheduleId);

        return schedule;
    }

    public async Task<List<Schedule>> GetByDate(DateOnly date)
    {
        return await context.Schedules
            .AsNoTracking()
            .Where(s => DateOnly.FromDateTime(s.StartTime) == date)
            .ToListAsync();
    }

    public async Task<List<Schedule>> GetByIds(List<Guid> ids)
    {
        return await context.Schedules
            .AsNoTracking()
            .Where(s => ids.Contains(s.Id))
            .ToListAsync();
    }
}
