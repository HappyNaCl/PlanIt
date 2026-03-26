using PlanIt.Domain.Common.Enums;

namespace PlanIt.Application.Common.Interfaces.Authentication;

public interface IRefreshTokenGenerator
{
    public string GenerateRefreshToken(Guid userId, string email, UserRole role);
}