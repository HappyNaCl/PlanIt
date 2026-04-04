using System.Net;
using PlanIt.Domain.Common.Exceptions;

namespace PlanIt.Domain.Common.Exceptions.Registrants;

public class AttractionBusyException(Guid attractionId)
    : ApiException(HttpStatusCode.Conflict, $"Attraction {attractionId} is currently busy, please try again.") { }
