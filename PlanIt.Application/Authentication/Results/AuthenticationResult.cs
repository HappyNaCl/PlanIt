using PlanIt.Domain.Common.Enums;

namespace PlanIt.Application.Authentication.Results;

public record AuthenticationResult(
    Guid Id,
    string Username,
    string Email,
    UserRole Role,
    string AccessToken,
    string RefreshToken);
