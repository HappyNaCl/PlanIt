using MediatR;
using PlanIt.Application.Authentication.Results;
using PlanIt.Application.Common.Interfaces.Authentication;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Domain.Common.Enums;
using PlanIt.Domain.Entities;

namespace PlanIt.Application.Authentication.Commands.Register;

public class RegisterCommandHandler (
        IAccessTokenGenerator accessTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher
    ) : IRequestHandler<RegisterCommand, AuthenticationResult>
{
    public async Task<AuthenticationResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var hashedPassword = passwordHasher.Hash(request.Password);

        var newUser = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = hashedPassword,
            Role = UserRole.USER
        };

        var savedUser = await userRepository.CreateUser(newUser);

        var accessToken = accessTokenGenerator.GenerateAccessToken(
            savedUser.Id, savedUser.Email, savedUser.Role);
        var refreshToken = refreshTokenGenerator.GenerateRefreshToken(
            savedUser.Id);

        return new AuthenticationResult
        (
            savedUser.Id,
            savedUser.Username,
            savedUser.Email,
            accessToken,
            refreshToken
        );
    }
}