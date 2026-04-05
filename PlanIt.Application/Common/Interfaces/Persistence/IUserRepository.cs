using PlanIt.Domain.Entities;

namespace PlanIt.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    public Task<User> Create(User user);
    public Task<User> GetById(Guid id);
    public Task<User?> GetByUsernameDefault(string username);
    public Task<int> CountAsync();
}