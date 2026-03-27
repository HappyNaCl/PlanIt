namespace PlanIt.Application.Authentication.Results;

public record AuthenticationResult(
    Guid UserId,
    string Username,
    string Email,
    string AccessToken,
    string RefreshToken
);
