using AppHost;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var rabbitUser = builder.AddParameter("rabbitUsername", true);
var rabbitPassword = builder.AddParameter("rabbitPassword", true);
var minioUser = builder.AddParameter("minioUsername", true);
var minioPassword = builder.AddParameter("minioPassword", true);
var arcadeRootPassword = builder.AddParameter("arcadeDbRootPassword", true);

const StartupOptions startup = StartupOptions.All;

var blobStorage = builder
    .AddBlobStorage(minioUser, minioPassword, startup);

var rabbitMq = builder
    .AddMessaging(rabbitUser, rabbitPassword, startup);

var graphDb = builder
    .AddGraphDatabase(arcadeRootPassword, startup);

var web = builder
    .AddProject<Ahead_Web>("Frontend")
    .ReferenceAndWaitForMessagingIfAvailable(rabbitMq, rabbitUser, rabbitPassword);

if (blobStorage != null)
    web.WaitFor(blobStorage);

if (graphDb.HasValue)
{
    var (
        graphDbContainer, 
        connectionString) = graphDb.Value;
    web.WithReference(connectionString);
    web.WaitFor(graphDbContainer);
}

if (startup.HasFlag(StartupOptions.Backend))
{
    builder
        .AddProject<Ahead_Backend>("Backend")
        .ReferenceAndWaitForMessagingIfAvailable(rabbitMq, rabbitUser, rabbitPassword);
}

builder.Build().Run();