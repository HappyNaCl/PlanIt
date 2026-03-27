using System.Net;

namespace PlanIt.Domain.Common.Exceptions.Users;

public class UserNotFoundException(string identifier)
    : ApiException(HttpStatusCode.NotFound, $"User {identifier} is not found.") { }