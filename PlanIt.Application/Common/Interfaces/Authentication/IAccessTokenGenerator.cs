using PlanIt.Domain.Common.Enums;

namespace PlanIt.Application.Common.Interfaces.Authentication;

public interface IAccessTokenGenerator
{
    public string GenerateAccessToken(Guid userId, string email, UserRole role);
    
    public Guid ValidateAccessToken(string token);
}