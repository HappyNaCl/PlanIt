namespace PlanIt.Contracts.Authentication.Request;

public record LoginRequest(
    string Username,
    string Password
    );
