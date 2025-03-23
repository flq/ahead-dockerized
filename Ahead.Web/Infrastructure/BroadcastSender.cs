using System.Diagnostics;
using System.Text.Json;
using Ahead.Common;
using RabbitMQ.Client;

namespace Ahead.Web.Infrastructure;

public class BroadcastSender(IConnection connection, ILogger<BroadcastSender> logger)
{
    public async Task Send<T>(string broadcastExchange, T message)
    {
        var parentContext = Activity.Current?.Context ?? default;
        using var activity = OTelUtilities.MessagingActivitySource.StartActivity($"{broadcastExchange} send", ActivityKind.Producer, parentContext);
        await using var channel = await connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(exchange: broadcastExchange, type: ExchangeType.Fanout);
        
        var body = JsonSerializer.SerializeToUtf8Bytes(message, options: SerializationUtilities.Options);

        var properties = new BasicProperties
        {
            Headers = new Dictionary<string, object?>()
        };

        activity.TryInjectTraceContextIntoDictionary(properties.Headers);

        await channel.BasicPublishAsync(
            new PublicationAddress(ExchangeType.Fanout, broadcastExchange, string.Empty), 
            properties, 
            body);
        logger.LogInformation("Sent message of type {messageType} on exchange {exchangeName}", typeof(T).Name, broadcastExchange);
    }
}