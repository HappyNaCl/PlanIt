using System.Net;

namespace PlanIt.Domain.Common.Exceptions.Authentication;

public class InvalidRefreshTokenException()
    : ApiException(HttpStatusCode.InternalServerError, "Invalid Refresh Token!") { }