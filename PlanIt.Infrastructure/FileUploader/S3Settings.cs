namespace PlanIt.Infrastructure.FileUploader;

public class S3Settings
{
    public const string SectionName = "S3";

    public string Endpoint { get; init; } = null!;
    public string AccessKey { get; init; } = null!;
    public string SecretKey { get; init; } = null!;
    public string BucketName { get; init; } = null!;
}