using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using PlanIt.Application.Common.Interfaces.FileUploader;

namespace PlanIt.Infrastructure.FileUploader;

public class S3Uploader(
    IAmazonS3 s3Client,
    IOptions<S3Settings> settings
    ) : IFileUploader
{
    private readonly S3Settings _settings = settings.Value;

    public string GetEndpoint() => $"{_settings.PublicEndpoint}/{_settings.BucketName}/";

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, string prefix)
    {
        var extension = Path.GetExtension(fileName);
        var key = $"{prefix}/{Guid.NewGuid()}{extension}";

        var request = new PutObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = key,
            InputStream = stream,
            ContentType = contentType,
            // DisablePayloadSigning = true
        };

        await s3Client.PutObjectAsync(request);

        return key;
    }

    public async Task<Stream> DownloadAsync(string key)
    {
        var request = new GetObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = key
        };

        var response = await s3Client.GetObjectAsync(request);

        return response.ResponseStream;
    }

    public async Task DeleteAsync(string key)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = key
        };

        await s3Client.DeleteObjectAsync(request);
    }
}
