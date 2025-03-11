using Ahead.Web;

var builder = WebApplication.CreateBuilder(args);
builder.AddRabbitMQClient(connectionName: "messaging");
builder.Services.AddSingleton<MessagesSender>();
var app = builder.Build();

app.MapGet("/", async (MessagesSender sender) =>
{
    await sender.Send("Hello World!");
    return Results.Text("Sent Hello World!");
});

app.Run();