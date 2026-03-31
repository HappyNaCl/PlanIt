using PlanIt.Domain.Common.Enums;

namespace PlanIt.Application.Me.Results;

public record MeResult(
    Guid Id,
    string Username,
    string Email,
    UserRole Role);