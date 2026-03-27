using System.Net;

namespace PlanIt.Domain.Common.Exceptions.Validation;

public class ValidationException(Dictionary<string, string[]> errors)
    : ApiException(HttpStatusCode.BadRequest, "One or more validation errors occurred.")
{
    public Dictionary<string, string[]> Errors { get; } = errors;
}