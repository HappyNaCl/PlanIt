using PlanIt.Application.Common.Interfaces.Datetime;

namespace PlanIt.Infrastructure.Datetime;

public class DatetimeProvider : IDatetimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}