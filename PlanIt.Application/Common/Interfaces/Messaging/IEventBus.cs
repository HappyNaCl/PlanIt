namespace PlanIt.Application.Common.Interfaces.Messaging;

public interface IEventBus
{
    Task PublishAsync<T>(T message, string queue);
}
