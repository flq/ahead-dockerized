using Ahead.Backend;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.AddRabbitMQClient(connectionName: "messaging");
builder.Services.AddTransient(typeof(QueueListener<>));
builder.Services.AddHostedService<Calculator>();
var app = builder.Build();
await app.RunAsync();