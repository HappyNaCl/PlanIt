using Microsoft.Extensions.Options;
using PlanIt.Application.Common.Interfaces.Authentication;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Domain.Entities;

namespace PlanIt.Infrastructure.Authentication;

public class RefreshTokenService(
    IOptions<TokenSettings> tokenOptions,
    IRefreshTokenRepository refreshTokenRepository)
    : IRefreshTokenGenerator, IRefreshTokenValidator
{
    private readonly TokenSettings _tokenSettings = tokenOptions.Value;

    public async Task<RefreshToken> GenerateRefreshToken(Guid userId)
    {
        var lifetime = TimeSpan.FromMinutes(_tokenSettings.RefreshExpiryMinutes);
        var token = new RefreshToken(userId, lifetime);
        return await refreshTokenRepository.Create(token);
    }

    public async Task<Guid> ValidateRefreshToken(string token)
    {
        var refreshToken = await refreshTokenRepository.GetByTokenForUpdate(token);

        refreshToken.MarkUsed();
        await refreshTokenRepository.Update(refreshToken);

        return refreshToken.UserId;
    }
}
