namespace PlanIt.Contracts.Authentication.Response;

public record AuthenticationResponse(
    Guid Id,
    string Username,
    string Email,
    string Role,
    string AccessToken,
    string RefreshToken
    );