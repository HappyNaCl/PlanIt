namespace PlanIt.Application.Common.Interfaces.Idempotency;

public interface IIdempotencyCommand
{
    string IdempotencyKey { get; }
}