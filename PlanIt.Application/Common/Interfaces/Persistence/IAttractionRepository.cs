using PlanIt.Domain.Entities;

namespace PlanIt.Application.Common.Interfaces.Persistence;

public interface IAttractionRepository
{
    public Task<List<Attraction>> GetByScheduleId(Guid scheduleId);
    public Task<int> GetRemainingCapacity(Guid attractionId);
}