namespace PlanIt.Contracts.Authentication.Request;

public record RegisterRequest(
    string Email,
    string Username,
    string Password,
    string ConfirmPassword
    );