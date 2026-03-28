namespace PlanIt.Contracts.Authentication;

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken);
