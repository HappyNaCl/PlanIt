using System.Net;

namespace PlanIt.Domain.Common.Exceptions.Users;

public class UserInvalidCredentialException() 
            : ApiException(HttpStatusCode.Unauthorized, "Invalid credentials!") { }