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

public class QueueListener<T>(IConnection connection, ILogger<QueueListener<T>> logger)
{
    public async IAsyncEnumerable<T> StartListening(string queueName,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var dotnetChannel = Channel.CreateUnbounded<T>();
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
        queueName,
        false,
        false,
        false,
        cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, ea) =>
        {
            try
            {
                var parentContext = ea.BasicProperties.Headers.TryExtractPropagationContext();
                Baggage.Current = parentContext.Baggage;
                using var activity = OTelUtilities.MessagingActivitySource.StartActivity($"{ea.RoutingKey} receive", ActivityKind.Consumer, parentContext.ActivityContext);

                var message = JsonSerializer.Deserialize<T>(ea.Body.Span, SerializationUtilities.Options);
                if (message == null)
                {
                    logger.LogWarning("Received null message on queue {queueName}", queueName);
                    return Task.CompletedTask;
                }
                dotnetChannel.Writer.TryWrite(message);
            }
            catch (JsonException x)
            {
                logger.LogError(x, "Received json exception on queue {queueName}", queueName);
            }
            return Task.CompletedTask;

        };
        _ = channel.BasicConsumeAsync(queueName, true, consumer, cancellationToken);
        logger.LogInformation("Listening for {queueName}", queueName);
        await foreach (var message in dotnetChannel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return message;
        }
    }
}