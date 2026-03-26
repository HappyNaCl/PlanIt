namespace PlanIt.Contracts.Authentication;

public record RegisterRequest(
    string Email,
    string Password,
    string Username,
    string ConfirmPassword
    );