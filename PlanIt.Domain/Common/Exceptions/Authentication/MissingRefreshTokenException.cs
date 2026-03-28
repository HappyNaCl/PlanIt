using System.Net;

namespace PlanIt.Domain.Common.Exceptions.Authentication;

public class MissingRefreshTokenException() : ApiException(HttpStatusCode.Unauthorized, "Missing refresh token.") { }