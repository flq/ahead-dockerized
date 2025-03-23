using Ahead.Common;
using Ahead.Web;
using Ahead.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureOpenTelemetry();
builder.AddRabbitMQClient(connectionName: "messaging");
builder.Services.AddSingleton<QueueSender>();
builder.Services.AddSingleton<BroadcastSender>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<Random>(_ => new Random());
var app = builder.Build();
app.MapAppRoutes();

app.Run();