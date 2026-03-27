using MediatR;
using PlanIt.Application.Authentication.Results;

namespace PlanIt.Application.Authentication.Commands;

public record RegisterCommand(
    string Email,
    string Username,
    string Password,
    string ConfirmPassword
    ) : IRequest<AuthenticationResult>;