using System.Net;
using PlanIt.Domain.Common.Exceptions;

namespace PlanIt.Domain.Common.Exceptions.Registrants;

public class NotRegisteredException(Guid userId, Guid attractionId)
    : ApiException(HttpStatusCode.NotFound, $"User {userId} is not registered for attraction {attractionId}.") { }