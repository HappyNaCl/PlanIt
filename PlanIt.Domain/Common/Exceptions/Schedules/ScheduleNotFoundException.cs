using System.Net;
using PlanIt.Domain.Common.Exceptions;

namespace PlanIt.Domain.Common.Exceptions.Schedules;

public class ScheduleNotFoundException(Guid id)
    : ApiException(HttpStatusCode.NotFound, $"Schedule {id} is not found.") { }