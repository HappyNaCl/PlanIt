using Microsoft.EntityFrameworkCore;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Domain.Common.Exceptions.Attractions;
using PlanIt.Domain.Entities;

namespace PlanIt.Infrastructure.Persistence;

public class AttractionRepository(
    IApplicationDbContext context
    ) : IAttractionRepository
{
    public async Task<Attraction> Create(Attraction attraction)
    {
        context.Attractions.Add(attraction);
        await context.SaveChangesAsync(CancellationToken.None);
        return attraction;
    }

    public async Task<Attraction> Delete(Guid attractionId)
    {
        var attraction = await context.Attractions
            .FirstOrDefaultAsync(a => a.Id == attractionId)
            ?? throw new AttractionNotFoundException(attractionId);

        context.Attractions.Remove(attraction);
        await context.SaveChangesAsync(CancellationToken.None);
        return attraction;
    }

    public async Task<Attraction> Update(Attraction attraction)
    {
        context.Attractions.Update(attraction);
        await context.SaveChangesAsync(CancellationToken.None);
        return attraction;  
    }

    public async Task<Attraction> GetById(Guid attractionId)
    {
        return await context.Attractions
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == attractionId)
            ?? throw new AttractionNotFoundException(attractionId);
    }

    public async Task<List<Attraction>> GetByScheduleId(Guid scheduleId)
    {
        return await context.Attractions
            .AsNoTracking()
            .Include(a => a.Registrants)
            .Where(a => a.ScheduleId == scheduleId)
            .ToListAsync();
    }

    public async Task<Attraction> GetByIdForUpdate(Guid attractionId)
    {
        var attraction = await context.Attractions
            .AsNoTracking()
            .Include(a => a.Registrants)
            .FirstOrDefaultAsync(a => a.Id == attractionId) ?? throw new AttractionNotFoundException(attractionId);

        return attraction;
    }

    public async Task<int> GetRemainingCapacity(Guid attractionId)
    {
        var attraction = await context.Attractions
            .AsNoTracking()
            .Select(a => new { a.Id, a.Capacity, RegistrantCount = a.Registrants.Count() })
            .FirstOrDefaultAsync(a => a.Id == attractionId)
            ?? throw new AttractionNotFoundException(attractionId);

        return attraction.Capacity - attraction.RegistrantCount;
    }
}