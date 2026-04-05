using PlanIt.Domain.Entities;

namespace PlanIt.Application.Common.Interfaces.Persistence;

public interface IScheduleRepository
{
    public Task<Schedule> Create(Schedule schedule);
    public Task<Schedule> Update(Schedule schedule);
    public Task<Schedule> Delete(Guid scheduleId);
    public Task<Schedule> GetById(Guid scheduleId);
    public Task<Schedule> GetByIdForUpdate(Guid scheduleId);
    public Task<List<Schedule>> GetByDate(DateOnly date);
    public Task<List<(Schedule Schedule, int AttractionCount)>> GetByDateRange(DateTime startUtc, DateTime endUtc);
    public Task<List<Schedule>> GetByIds(List<Guid> ids);
    public Task<int> CountAsync();
}