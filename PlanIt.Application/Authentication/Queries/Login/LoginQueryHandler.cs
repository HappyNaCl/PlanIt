using MediatR;
using PlanIt.Application.Authentication.Results;
using PlanIt.Application.Common.Interfaces.Authentication;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Domain.Common.Exceptions.Users;

namespace PlanIt.Application.Authentication.Queries.Login;

public class LoginQueryHandler(
    IAccessTokenGenerator accessTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher
    ) : IRequestHandler<LoginQuery, AuthenticationResult>
{
    public async Task<AuthenticationResult> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByUsernameDefault(request.Username);

        if (user is null || !passwordHasher.Validate(request.Password, user.Password))
            throw new UserInvalidCredentialException();

        var accessToken = accessTokenGenerator.GenerateAccessToken(
            user.Id, user.Email, user.Role);
        var refreshToken = await refreshTokenGenerator.GenerateRefreshToken(
            user.Id);

        return new AuthenticationResult
        (
            user.Id,
            user.Username,
            user.Email,
            accessToken,
            refreshToken.Token
        );
    }
}