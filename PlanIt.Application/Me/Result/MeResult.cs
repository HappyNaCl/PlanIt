using PlanIt.Domain.Common.Enums;

namespace PlanIt.Application.Me.Result;

public record MeResult(
    Guid Id,
    string Username,
    string Email,
    UserRole Role);