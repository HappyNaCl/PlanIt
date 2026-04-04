using System.Text.Json;
using PlanIt.Application.Common.Interfaces.Messaging;
using RabbitMQ.Client;

namespace PlanIt.Infrastructure.Messaging;

public class RabbitMqEventBus(IConnection connection) : IEventBus
{
    public async Task PublishAsync<T>(T message, string queue)
    {
        var channel = await connection.CreateChannelAsync();
        await using var _ = channel;

        await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var body = JsonSerializer.SerializeToUtf8Bytes(message);
        var props = new BasicProperties { Persistent = true };

        await channel.BasicPublishAsync(exchange: "", routingKey: queue, mandatory: false, basicProperties: props, body: body);
    }
}
