using System.Net;

namespace PlanIt.Domain.Common.Exceptions;

public abstract class ApiException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}