using System.Text.Json;
using MediatR;
using PlanIt.Application.Common.Interfaces.Idempotency;
using PlanIt.Application.Common.Interfaces.Stores;

namespace PlanIt.Application.Common.Behaviors;

public class IdempotencyBehavior<TRequest, TResponse>(
    IIdempotencyStore<string> store
    ) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly TimeSpan Expiry = TimeSpan.FromMinutes(30);

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IIdempotencyCommand idempotentCommand)
            return await next(cancellationToken);

        var key = $"idempotency:{idempotentCommand.IdempotencyKey}";

        var cached = await store.GetAsync(key);
        if (cached is not null)
        {
            if (typeof(TResponse) == typeof(Unit))
                return (TResponse)(object)Unit.Value;

            return JsonSerializer.Deserialize<TResponse>(cached)!;
        }

        var response = await next(cancellationToken);

        var serialized = typeof(TResponse) == typeof(Unit)
            ? "__unit__"
            : JsonSerializer.Serialize(response);

        await store.SaveAsync(key, serialized, Expiry);

        return response;
    }
}