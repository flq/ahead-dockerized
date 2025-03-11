using Ahead.Common;
using Ahead.Common.Dto;
using Ahead.Web;

var builder = WebApplication.CreateBuilder(args);
builder.AddRabbitMQClient(connectionName: "messaging");
builder.Services.AddSingleton<QueueSender>();
var app = builder.Build();

app.MapGet("/", async (QueueSender sender, string calculation = "Test") =>
{
    await sender.Send(Constants.QueueNames.Basic, new CalculationRequest { Calculation = calculation });
    return Results.Text("Sent calculation!");
});

app.Run();