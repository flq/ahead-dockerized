using Ahead.Backend;
using Ahead.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.ConfigureOpenTelemetry();
builder.AddRabbitMQClient(connectionName: "messaging");
builder.Services.AddTransient(typeof(QueueListener<>));
builder.Services.AddHostedService<ReportGenerator>();
var app = builder.Build();
await app.RunAsync();