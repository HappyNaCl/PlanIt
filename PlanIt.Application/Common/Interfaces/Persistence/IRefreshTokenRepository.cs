using PlanIt.Domain.Entities;

namespace PlanIt.Application.Common.Interfaces.Persistence;

public interface IRefreshTokenRepository
{
    public Task<RefreshToken> Create(RefreshToken refreshToken);
    public Task<RefreshToken> GetByTokenForUpdate(string token);
    public Task<bool> Delete(RefreshToken refreshToken);
    public Task Update(RefreshToken refreshToken);
}