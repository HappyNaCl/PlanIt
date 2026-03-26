using PlanIt.Application.Common.Interfaces.FileUploader;

namespace PlanIt.Infrastructure.FileUploader;

public class S3Uploader : IFileUploader
{
    public Task<string> UploadAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Stream> DownloadAsync()
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync()
    {
        throw new NotImplementedException();
    }
}