using PlanIt.Domain.Entities;

namespace PlanIt.Application.Common.Interfaces.Persistence;

public interface IAttractionRepository
{
    public Task<Attraction> Create(Attraction attraction);
    public Task<Attraction> Delete(Guid attractionId);
    public Task<Attraction> Update(Attraction attraction);
    public Task<Attraction> GetById(Guid attractionId);
    public Task<List<Attraction>> GetByScheduleId(Guid scheduleId);
    public Task<Attraction> GetByIdForUpdate(Guid attractionId);
    public Task<int> GetRemainingCapacity(Guid attractionId);
}