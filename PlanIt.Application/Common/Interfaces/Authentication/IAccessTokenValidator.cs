namespace PlanIt.Application.Common.Interfaces.Authentication;

public interface IAccessTokenValidator
{
    public Guid ValidateAccessToken(string token);
}