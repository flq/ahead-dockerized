using Ahead.Common;
using Ahead.Web.Endpoints;
using Ahead.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureOpenTelemetry();
builder.Services
    .AddRazorPages()
    .AddRazorRuntimeCompilation();

builder.AddQueueingAndBroadcasting();
builder.AddBlobStorage();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<Random>(_ => new Random());

var app = builder.Build();

app.MapApiRoutes();
app.MapRazorPages();
app.Run();