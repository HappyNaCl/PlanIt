using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlanIt.Application.Attractions.Results;
using PlanIt.Application.Common.Interfaces.FileUploader;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Application.Common.Interfaces.Realtime;
using PlanIt.Application.Registrants.Messages;
using PlanIt.Domain.Common.Exceptions.Registrants;
using PlanIt.Domain.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlanIt.Infrastructure.Messaging.Consumers;

public class JoinAttractionConsumer(
    IConnection connection,
    IServiceScopeFactory scopeFactory,
    IAttractionNotifier notifier
    ) : BackgroundService
{
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(JoinAttractionMessage.Queue, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);
        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var message = JsonSerializer.Deserialize<JoinAttractionMessage>(ea.Body.ToArray())!;
                await HandleAsync(message);
                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception)
            {
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        await _channel.BasicConsumeAsync(JoinAttractionMessage.Queue, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleAsync(JoinAttractionMessage message)
    {
        using var scope = scopeFactory.CreateScope();
        var attractionRepo = scope.ServiceProvider.GetRequiredService<IAttractionRepository>();
        var registrantRepo = scope.ServiceProvider.GetRequiredService<IRegistrantRepository>();
        var fileUploader = scope.ServiceProvider.GetRequiredService<IFileUploader>();

        var remaining = await attractionRepo.GetRemainingCapacity(message.AttractionId);
        if (remaining <= 0)
        {
            await notifier.SendRegistrationFailed(message.UserId.ToString(), "This attraction is full.");
            return;
        }

        try
        {
            Console.WriteLine($"[JoinAttractionConsumer]: {message.UserId} JOINING {message.AttractionId}");
            await registrantRepo.AddAsync(new Registrant
            {
                UserId = message.UserId,
                AttractionId = message.AttractionId
            });
        }
        catch (AlreadyRegisteredException)
        {
            // treat as success — idempotent
            Console.WriteLine($"[JoinAttractionConsumer]: IDEMPOTENT {message.UserId} JOINING {message.AttractionId}");
        }

        var attraction = await attractionRepo.GetById(message.AttractionId);
        var updatedRemaining = await attractionRepo.GetRemainingCapacity(message.AttractionId);

        var result = new AttractionResult(
            attraction.Id,
            attraction.ScheduleId,
            attraction.Name,
            attraction.Description,
            $"{fileUploader.GetEndpoint()}{attraction.ImageKey}",
            attraction.Capacity,
            updatedRemaining);

        await notifier.BroadcastCapacityUpdate(message.ScheduleId, message.AttractionId, updatedRemaining);
        await notifier.SendRegistrationConfirmed(message.UserId.ToString(), result);
    }

    public override void Dispose()
    {
        _channel?.CloseAsync().GetAwaiter().GetResult();
        _channel?.Dispose();
        base.Dispose();
    }
}
