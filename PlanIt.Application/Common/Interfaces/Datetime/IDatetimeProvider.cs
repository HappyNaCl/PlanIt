namespace PlanIt.Application.Common.Interfaces.Datetime;

public interface IDatetimeProvider
{
    DateTime UtcNow { get; }
}