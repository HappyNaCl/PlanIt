namespace PlanIt.Application.Common.Interfaces.FileUploader;

public interface IFileUploader
{
    public Task<string> UploadAsync(Stream stream, string fileName, string contentType, string prefix);
    public Task<Stream> DownloadAsync(string key);
    public Task DeleteAsync(string key);
}