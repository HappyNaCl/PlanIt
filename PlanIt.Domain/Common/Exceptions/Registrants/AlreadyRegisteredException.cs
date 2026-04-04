using System.Net;
using PlanIt.Domain.Common.Exceptions;

namespace PlanIt.Domain.Common.Exceptions.Registrants;

public class AlreadyRegisteredException(Guid userId, Guid attractionId)
    : ApiException(HttpStatusCode.Conflict, $"User {userId} is already registered for attraction {attractionId}.") { }