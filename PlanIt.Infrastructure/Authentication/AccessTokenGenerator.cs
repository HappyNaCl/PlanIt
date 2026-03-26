using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PlanIt.Application.Common.Interfaces.Authentication;
using PlanIt.Application.Common.Interfaces.Datetime;
using PlanIt.Domain.Common.Enums;

namespace PlanIt.Infrastructure.Authentication;

public class AccessTokenGenerator(IOptions<JwtSettings> jwtOptions, IDatetimeProvider dateTimeProvider)
    : IAccessTokenGenerator
{
    private readonly JwtSettings jwtSettings = jwtOptions.Value;
    public string GenerateAccessToken(Guid userId, string email, UserRole role)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.AccessTokenSecret)
            ), SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(dateTimeProvider.UtcNow).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new Claim("Role", role.ToString())
        };
        
        var accessToken = new JwtSecurityToken(
            jwtSettings.Issuer,
            jwtSettings.Audience,
            claims,
            expires: dateTimeProvider.UtcNow.AddMinutes(jwtSettings.AccessExpiryMinutes),
            signingCredentials: signingCredentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(accessToken);
    }
}