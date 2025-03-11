using System.Text;
using RabbitMQ.Client;

namespace Ahead.Web;

public class MessagesSender(IConnection connection)
{
    public async Task Send(string message)
    {
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(
            queue: "hello", 
            durable: false, 
            exclusive: false, 
            autoDelete: false, 
            arguments: null);
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "hello", body: body);
    }
    
}