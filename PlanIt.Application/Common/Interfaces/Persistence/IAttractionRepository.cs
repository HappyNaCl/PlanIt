using PlanIt.Domain.Entities;

namespace PlanIt.Application.Common.Interfaces.Persistence;

public interface IAttractionRepository
{
    public Task<Attraction> Create(Attraction attraction);
    public Task<Attraction> Delete(Guid attractionId);
    public Task<List<Attraction>> GetByScheduleId(Guid scheduleId);
    public Task<int> GetRemainingCapacity(Guid attractionId);
}