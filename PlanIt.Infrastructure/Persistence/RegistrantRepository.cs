using Microsoft.EntityFrameworkCore;
using Npgsql;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Application.Registrants.Results;
using PlanIt.Domain.Common.Exceptions.Registrants;
using PlanIt.Domain.Entities;

namespace PlanIt.Infrastructure.Persistence;

public class RegistrantRepository(IApplicationDbContext context) : IRegistrantRepository
{
    public async Task<Registrant> AddAsync(Registrant registrant)
    {
        try
        {
            context.Registrants.Add(registrant);
            await context.SaveChangesAsync(CancellationToken.None);
            return registrant;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
        {
            throw new AlreadyRegisteredException(registrant.UserId, registrant.AttractionId);
        }
    }

    public async Task RemoveAsync(Guid userId, Guid attractionId)
    {
        var registrant = await context.Registrants
            .FirstOrDefaultAsync(r => r.UserId == userId && r.AttractionId == attractionId)
            ?? throw new NotRegisteredException(userId, attractionId);

        registrant.Leave();
        context.Registrants.Remove(registrant);
        await context.SaveChangesAsync(CancellationToken.None);
    }

    public async Task<ISet<Guid>> GetRegisteredAttractionIds(Guid userId)
    {
        var ids = await context.Registrants
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .Select(r => r.AttractionId)
            .ToListAsync();

        return ids.ToHashSet();
    }

    public async Task<List<MyAttractionResult>> GetUserAttractionsWithDetails(Guid userId)
    {
        return await context.Registrants
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .Join(context.Attractions,
                r => r.AttractionId,
                a => a.Id,
                (r, a) => new { a })
            .Join(context.Schedules,
                ra => ra.a.ScheduleId,
                s => s.Id,
                (ra, s) => new MyAttractionResult(
                    ra.a.Id,
                    ra.a.Name,
                    ra.a.Description,
                    ra.a.ImageKey,
                    ra.a.Capacity,
                    ra.a.Capacity - ra.a.Registrants.Count(),
                    s.Id,
                    s.Name,
                    s.Location,
                    s.StartTime,
                    s.EndTime))
            .ToListAsync();
    }

    public Task<int> CountAsync() => context.Registrants.CountAsync();
}