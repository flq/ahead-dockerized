using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ahead.Backend;

public class QueueListener<T>(IConnection connection, ILogger<QueueListener<T>> logger)
{
    private readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false,
    };
    
    public async IAsyncEnumerable<T> StartListening(string queueName, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var dotnetChannel = Channel.CreateUnbounded<T>();
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
        queue: queueName, 
        durable: false, 
        exclusive: false, 
        autoDelete: false, 
        arguments: null, 
        cancellationToken: cancellationToken);
        
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, ea) =>
        {
            try
            {
                var message = JsonSerializer.Deserialize<T>(ea.Body.Span, options);
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
        _ = channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer, cancellationToken: cancellationToken);
        logger.LogInformation("Listening for {queueName}", queueName);
        await foreach (var message in dotnetChannel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return message;
        }
    }
}