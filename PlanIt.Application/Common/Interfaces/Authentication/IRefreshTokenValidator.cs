namespace PlanIt.Application.Common.Interfaces.Authentication;

public interface IRefreshTokenValidator
{
    Task<Guid> ValidateRefreshToken(string token);
}