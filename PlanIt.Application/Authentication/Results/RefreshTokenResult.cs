namespace PlanIt.Application.Authentication.Results;

public record RefreshTokenResult(
    string AccessToken,
    string RefreshToken);
