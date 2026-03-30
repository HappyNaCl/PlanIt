using Microsoft.EntityFrameworkCore;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Domain.Common.Exceptions.Attractions;
using PlanIt.Domain.Entities;

namespace PlanIt.Infrastructure.Persistence;

public class AttractionRepository(
    IApplicationDbContext context
    ) : IAttractionRepository
{
    public async Task<List<Attraction>> GetByScheduleId(Guid scheduleId)
    {
        return await context.Attractions
            .AsNoTracking()
            .Include(a => a.Registrants)
            .Where(a => a.ScheduleId == scheduleId)
            .ToListAsync();
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