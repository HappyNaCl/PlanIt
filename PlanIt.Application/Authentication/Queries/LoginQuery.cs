using MediatR;
using PlanIt.Application.Authentication.Results;

namespace PlanIt.Application.Authentication.Queries;

public record LoginQuery(
    string Username,
    string Password
    ) : IRequest<AuthenticationResult>;