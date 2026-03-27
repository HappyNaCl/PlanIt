using MediatR;
using PlanIt.Application.Authentication.Results;

namespace PlanIt.Application.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken) : IRequest<RefreshTokenResult>;
