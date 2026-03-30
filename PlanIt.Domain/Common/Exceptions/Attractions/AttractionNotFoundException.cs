using System.Net;
using PlanIt.Domain.Common.Exceptions;

namespace PlanIt.Domain.Common.Exceptions.Attractions;

public class AttractionNotFoundException(Guid id)
    : ApiException(HttpStatusCode.NotFound, $"Attraction {id} is not found.") { }