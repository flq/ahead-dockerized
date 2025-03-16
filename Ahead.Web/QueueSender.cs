using System.Text.Json;
using System.Text.Json.Serialization;
using RabbitMQ.Client;

namespace Ahead.Web;

public class QueueSender(IConnection connection, ILogger<QueueSender> logger)
{
    private readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false,
    };
    
    public async Task Send<T>(string queueName, T message)
    {
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(
            queue: queueName, 
            durable: false, 
            exclusive: false, 
            autoDelete: false, 
            arguments: null);
        
        var body = JsonSerializer.SerializeToUtf8Bytes(message, options: options);
        
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);
        logger.LogInformation("Sent message of type {messageType} on queue {queueName}", typeof(T).Name, queueName);
    }
    
}