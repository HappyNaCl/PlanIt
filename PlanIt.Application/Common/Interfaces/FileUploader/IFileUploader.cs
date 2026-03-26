namespace PlanIt.Application.Common.Interfaces.FileUploader;

public interface IFileUploader
{
    public Task<string> UploadAsync();
    public Task<Stream> DownloadAsync();
    public Task DeleteAsync();
}