using Ahead.Web;

var builder = WebApplication.CreateBuilder(args);
builder.AddRabbitMQClient(connectionName: "messaging");
builder.Services.AddSingleton<QueueSender>();
builder.Services.AddSingleton(TimeProvider.System);
var app = builder.Build();
app.MapAppRoutes();

app.Run();