using PlanIt.Domain.Common.Models;

namespace PlanIt.Domain.ValueObjects;

public class ImageFile : ValueObject
{
    public Stream Content { get; }
    public string FileName { get; }
    public string ContentType { get; }

    public ImageFile(Stream content, string fileName, string contentType)
    {
        Content = content;
        FileName = fileName;
        ContentType = contentType;
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return FileName;
        yield return ContentType;
    }
}
