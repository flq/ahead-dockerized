using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;
using Ahead.Common;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ahead.Backend.Infrastructure;

public class BroadcastListener<T>(IConnection connection, ILogger<BroadcastListener<T>> logger)
{
    public async IAsyncEnumerable<T> StartListening(
        string broadcastExchange,
        string? callerId = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var dotnetChannel = Channel.CreateUnbounded<T>();
        
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await channel.ExchangeDeclareAsync(exchange: broadcastExchange, type: ExchangeType.Fanout, cancellationToken: cancellationToken);
        
        var tmpQueue = await channel.QueueDeclareAsync(cancellationToken: cancellationToken);
        await channel.QueueBindAsync(queue: tmpQueue.QueueName, exchange: broadcastExchange, routingKey: string.Empty, cancellationToken: cancellationToken);
        
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, ea) =>
        {
            try
            {
                var parentContext = ea.BasicProperties.Headers.TryExtractPropagationContext();
                Baggage.Current = parentContext.Baggage;
                using var activity = OTelUtilities.MessagingActivitySource.StartActivity($"{ea.RoutingKey}{(callerId != null ? $" -> {callerId}" : "")} receive", ActivityKind.Consumer, parentContext.ActivityContext);

                var message = JsonSerializer.Deserialize<T>(ea.Body.Span, SerializationUtilities.Options);
                if (message == null)
                {
                    logger.LogWarning("Received null message on queue {queueName}", broadcastExchange);
                    return Task.CompletedTask;
                }
                dotnetChannel.Writer.TryWrite(message);
            }
            catch (JsonException x)
            {
                logger.LogError(x, "Received json exception on queue {queueName}", broadcastExchange);
            }
            return Task.CompletedTask;

        };
        _ = channel.BasicConsumeAsync(tmpQueue.QueueName, true, consumer, cancellationToken);
        logger.LogInformation("Listening for broadcasts on {exchangeName}", broadcastExchange);
        await foreach (var message in dotnetChannel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return message;
        }
        channel.Dispose();
    }
}