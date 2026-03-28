using Microsoft.EntityFrameworkCore;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Domain.Common.Exceptions.Authentication;
using PlanIt.Domain.Entities;

namespace PlanIt.Infrastructure.Persistence;

public class RefreshTokenRepository(IApplicationDbContext context) : IRefreshTokenRepository
{
    public async Task<RefreshToken> Create(RefreshToken refreshToken)
    {
        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync(CancellationToken.None);
        return refreshToken;
    }

    public async Task<RefreshToken> GetByTokenForUpdate(string token)
    {
        var refreshToken = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
        return refreshToken ?? throw new InvalidRefreshTokenException();
    }

    public async Task Update(RefreshToken refreshToken)
    {
        context.RefreshTokens.Update(refreshToken);
        await context.SaveChangesAsync(CancellationToken.None);
    }

    public async Task<bool> Delete(RefreshToken refreshToken)
    {
        context.RefreshTokens.Remove(refreshToken);
        var affected = await context.SaveChangesAsync(CancellationToken.None);
        return affected > 0;
    }
}
