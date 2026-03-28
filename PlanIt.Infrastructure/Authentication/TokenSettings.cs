namespace PlanIt.Infrastructure.Authentication;

public class TokenSettings
{
    public const string SectionName = "TokenSettings";
    
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } =  null!;
    public int AccessExpiryMinutes { get; init; }
    public int RefreshExpiryMinutes { get; init; }
    public string AccessTokenSecret  { get; init; } = null!;
}