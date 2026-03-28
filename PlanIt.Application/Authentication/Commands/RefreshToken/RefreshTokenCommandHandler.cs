using MediatR;
using PlanIt.Application.Authentication.Results;
using PlanIt.Application.Common.Interfaces.Authentication;
using PlanIt.Application.Common.Interfaces.Persistence;

namespace PlanIt.Application.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IAccessTokenGenerator accessTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IRefreshTokenValidator refreshTokenValidator,
    IUserRepository userRepository
    ) : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
{
    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = await refreshTokenValidator.ValidateRefreshToken(request.RefreshToken);

        var user = await userRepository.GetById(userId);

        var accessToken = accessTokenGenerator.GenerateAccessToken(
            user.Id, user.Email, user.Role);

        var refreshToken = await refreshTokenGenerator.GenerateRefreshToken(user.Id);

        return new RefreshTokenResult(accessToken, refreshToken.Token);
    }
}