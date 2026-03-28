using System.Security.Cryptography;
using PlanIt.Domain.Common.Exceptions.Authentication;
using PlanIt.Domain.Common.Models;

namespace PlanIt.Domain.Entities;

public class RefreshToken : Entity<Guid>
{
    public RefreshToken() : base(Guid.NewGuid()) {}
    
    public RefreshToken(Guid userId, TimeSpan lifetime) : base(Guid.NewGuid())
    {
        UserId = userId;
        Token = GenerateTokenString();
        IsUsed = false;
        ExpiresAt = DateTime.UtcNow.Add(lifetime);
    }

    public Guid UserId { get; init; }
    public string Token { get; init; }
    public bool IsUsed { get; set; }
    public DateTime ExpiresAt { get; init; }

    public User User { get; private set; } = null!;
    
    public bool IsValid()
        => !IsUsed && ExpiresAt > DateTime.UtcNow;
    
    public void MarkUsed()
    {
        if (IsUsed)    
            throw new ReusedRefreshTokenException();
        if (!IsValid())
            throw new InvalidRefreshTokenException(); 
        
        IsUsed = true;
    }

    private static string GenerateTokenString()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "")
            [..50];
    }
}