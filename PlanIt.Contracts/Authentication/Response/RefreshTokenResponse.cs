namespace PlanIt.Contracts.Authentication.Response;

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken);
