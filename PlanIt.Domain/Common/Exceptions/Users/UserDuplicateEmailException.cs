using System.Net;

namespace PlanIt.Domain.Common.Exceptions.Users;

public class UserDuplicateEmailException() 
    : ApiException(HttpStatusCode.BadRequest, "This email is already taken.") {}