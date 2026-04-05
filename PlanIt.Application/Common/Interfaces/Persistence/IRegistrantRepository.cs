using PlanIt.Domain.Entities;

namespace PlanIt.Application.Common.Interfaces.Persistence;

public interface IRegistrantRepository
{
    Task<Registrant> AddAsync(Registrant registrant);
    Task RemoveAsync(Guid userId, Guid attractionId);
    Task<ISet<Guid>> GetRegisteredAttractionIds(Guid userId);
    Task<int> CountAsync();
}