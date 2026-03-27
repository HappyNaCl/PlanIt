using PlanIt.Domain.Entities;

namespace PlanIt.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    public Task<User> CreateUser(User user);
    public Task<User> GetUser(Guid id);
    public Task<User> GetUserByUsername(string username);
}