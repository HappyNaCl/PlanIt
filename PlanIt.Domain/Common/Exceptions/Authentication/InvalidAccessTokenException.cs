using System.Net;

namespace PlanIt.Domain.Common.Exceptions.Authentication;

public class InvalidAccessTokenException()
    : ApiException(HttpStatusCode.InternalServerError, "Invalid Access Token.") { }