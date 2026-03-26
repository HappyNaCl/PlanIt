namespace PlanIt.Infrastructure.Authentication;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";
    
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } =  null!;
    public int AccessExpiryMinutes { get; init; }
    public int RefreshExpiryMinutes { get; init; }
    public string AccessTokenSecret  { get; init; } = null!;
    public string RefreshTokenSecret { get; init; } = null!;
}