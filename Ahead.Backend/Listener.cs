using System.Text;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ahead.Backend;

public class Listener(IConnection connection) : IHostedService
{
    private IChannel? channel;
    
    public async Task StartAsync(CancellationToken token)
    {
        channel = await connection.CreateChannelAsync(cancellationToken: token);

        await channel.QueueDeclareAsync(
            queue: "hello", 
            durable: false, 
            exclusive: false, 
            autoDelete: false, 
            arguments: null, 
            cancellationToken: token);
        
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(message);
            return Task.CompletedTask;
        };
        await channel.BasicConsumeAsync("hello", autoAck: true, consumer: consumer, cancellationToken: token);

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        channel?.Dispose();
        return Task.CompletedTask;
    }
}