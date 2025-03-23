using Ahead.Backend;
using Ahead.Backend.Infrastructure;
using Ahead.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.ConfigureOpenTelemetry();
builder.AddRabbitMQClient(connectionName: "messaging");
builder.Services.AddTransient(typeof(QueueListener<>));
builder.Services.AddTransient(typeof(BroadcastListener<>));

builder.Services.AddHostedService<ReportGenerator>();
builder.Services.AddHostedService<SearchIndexUpdater>();
builder.Services.AddHostedService<NotificationGenerator>();

var app = builder.Build();
await app.RunAsync();