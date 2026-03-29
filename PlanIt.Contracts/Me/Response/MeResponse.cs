namespace PlanIt.Contracts.Me.Response;

public record MeResponse(
    Guid Id,
    string Username,
    string Email,
    string Role);