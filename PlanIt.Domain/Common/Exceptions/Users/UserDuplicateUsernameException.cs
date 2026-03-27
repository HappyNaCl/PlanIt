using System.Net;

namespace PlanIt.Domain.Common.Exceptions.Users;

public class UserDuplicateUsernameException() :
        ApiException(HttpStatusCode.Conflict, "This username is already taken.") {}