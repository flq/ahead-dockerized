
namespace Ahead.Web.Infrastructure;

public static class StartupExtensions
{
    public static void AddQueueingAndBroadcasting(this WebApplicationBuilder builder)
    {
        builder.AddRabbitMQClient(connectionName: "messaging");
        builder.Services.AddSingleton<QueueSender>();
        builder.Services.AddSingleton<BroadcastSender>();
    }
    
    public static void AddBlobStorage(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<MinioConfig>(builder.Configuration.GetSection("BlobStorage"));
        builder.Services.AddScoped<IBlobStorage, MinioBlobStorage>();
    }
}