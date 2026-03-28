using PlanIt.Domain.Entities;

namespace PlanIt.Application.Common.Interfaces.Authentication;

public interface IRefreshTokenGenerator
{
    Task<RefreshToken> GenerateRefreshToken(Guid userId);
}