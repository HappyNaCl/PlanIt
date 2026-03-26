using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PlanIt.Application.Common.Interfaces.Authentication;
using PlanIt.Application.Common.Interfaces.Datetime;
using PlanIt.Domain.Common.Enums;

namespace PlanIt.Infrastructure.Authentication;

public class RefreshTokenGenerator(IOptions<JwtSettings> jwtOptions, IDatetimeProvider datetimeProvider)
    : IRefreshTokenGenerator
{
    private readonly JwtSettings jwtSettings = jwtOptions.Value;
    public string GenerateRefreshToken(Guid userId, string email, UserRole role)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.RefreshTokenSecret)
            ), SecurityAlgorithms.HmacSha256
        );
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(datetimeProvider.UtcNow).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
        };

        var refreshToken = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            expires: datetimeProvider.UtcNow.AddMinutes(jwtSettings.RefreshExpiryMinutes),
            claims: claims,
            signingCredentials: signingCredentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(refreshToken);
    }
}