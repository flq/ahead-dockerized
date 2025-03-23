using System.Diagnostics;
using System.Text.Json;
using Ahead.Common;
using RabbitMQ.Client;

namespace Ahead.Web;

public class QueueSender(IConnection connection, ILogger<QueueSender> logger)
{
    public async Task Send<T>(string queueName, T message)
    {
        var parentContext = Activity.Current?.Context ?? default;
        using var activity = OTelUtilities.MessagingActivitySource.StartActivity($"{queueName} send", ActivityKind.Producer, parentContext);
        
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(
            queue: queueName, 
            durable: false, 
            exclusive: false, 
            autoDelete: false, 
            arguments: null);
        
        var body = JsonSerializer.SerializeToUtf8Bytes(message, options: SerializationUtilities.Options);

        var properties = new BasicProperties
        {
            Headers = new Dictionary<string, object?>()
        };

        activity.TryInjectTraceContextIntoDictionary(properties.Headers);

        await channel.BasicPublishAsync(
            new PublicationAddress(string.Empty, string.Empty, queueName), 
            properties, 
            body);
        logger.LogInformation("Sent message of type {messageType} on queue {queueName}", typeof(T).Name, queueName);
    }
    
}