namespace PlanIt.Contracts.Authentication;

public record RegisterRequest(
    string Email,
    string Username,
    string Password,
    string ConfirmPassword
    );