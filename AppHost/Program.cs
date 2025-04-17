using AppHost;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var rabbitUser = builder.AddParameter("rabbitUsername", true);
var rabbitPassword = builder.AddParameter("rabbitPassword", true);
var minioUser = builder.AddParameter("minioUsername", true);
var minioPassword = builder.AddParameter("minioPassword", true);

var startup = StartupOptions.All;

var blobStorage = builder
    .AddBlobStorage(minioUser, minioPassword, startup);

var rabbitMq = builder
    .AddMessaging(rabbitUser, rabbitPassword, startup);

var web = builder.AddProject<Ahead_Web>("Frontend");

if (rabbitMq != null)
{
    web.WithReference(rabbitMq)
        .WaitFor(rabbitMq)
        .WithEnvironment("RABBITMQ_USERNAME", rabbitUser)
        .WithEnvironment("RABBITMQ_PASSWORD", rabbitPassword);
}

if (blobStorage != null)
    web.WaitFor(blobStorage);

if (startup.HasFlag(StartupOptions.Backend))
{
    var backend = builder.AddProject<Ahead_Backend>("Backend");
    if (rabbitMq != null)
    {
        backend.WithReference(rabbitMq)
            .WithEnvironment("RABBITMQ_USERNAME", rabbitUser)
            .WithEnvironment("RABBITMQ_PASSWORD", rabbitPassword)
            .WaitFor(rabbitMq);
    }
}

builder.Build().Run();