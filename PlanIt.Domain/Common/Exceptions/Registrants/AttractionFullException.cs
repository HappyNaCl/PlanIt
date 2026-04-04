using System.Net;
using PlanIt.Domain.Common.Exceptions;

namespace PlanIt.Domain.Common.Exceptions.Registrants;

public class AttractionFullException(Guid attractionId)
    : ApiException(HttpStatusCode.Conflict, $"Attraction {attractionId} is full.") { }