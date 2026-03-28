using System.Net;

namespace PlanIt.Domain.Common.Exceptions.Authentication;

public class ReusedRefreshTokenException() : ApiException(HttpStatusCode.Conflict, "Refresh Token has already been used.") { }