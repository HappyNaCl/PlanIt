using PlanIt.Domain.Common.Enums;

namespace PlanIt.Application.Common.Interfaces.Authentication;

public interface IRefreshTokenGenerator
{
    public string GenerateRefreshToken(Guid userId);
    
    public Guid ValidateRefreshToken(string token);
}