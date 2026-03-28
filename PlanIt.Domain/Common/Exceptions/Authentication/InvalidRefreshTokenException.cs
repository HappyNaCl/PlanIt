using System.Net;

namespace PlanIt.Domain.Common.Exceptions.Authentication;

public class InvalidRefreshTokenException()
    : ApiException(HttpStatusCode.Unauthorized, "Invalid or expired refresh token.") { }