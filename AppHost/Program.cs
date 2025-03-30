using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var rabbitUser = builder.AddParameter("rabbitUsername", secret: true);
var rabbitPassword = builder.AddParameter("rabbitPassword", secret: true);
var minioUser = builder.AddParameter("minioUsername", secret: true);
var minioPassword = builder.AddParameter("minioPassword", secret: true);

var blobStorage = builder
    .AddContainer("minio", "minio/minio")
    .WithContainerName("ahead_blobstorage")
    .WithEnvironment("MINIO_ROOT_USER", minioUser)
    .WithEnvironment("MINIO_ROOT_PASSWORD", minioPassword)
    .WithEndpoint(9000, 9000, scheme: "http", name: "minioserver")
    .WithEndpoint(9090, 9090, scheme: "http", name: "minioconsole")
    .WithBindMount("../data/minio", "/data")
    .WithArgs("server", "/data", "--console-address", ":9090");

var rabbitMq = builder
    .AddRabbitMQ("messaging", rabbitUser, rabbitPassword)
    .WithManagementPlugin();

builder.AddProject<Ahead_Web>("Frontend")
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WaitFor(blobStorage)
    .WithEnvironment("RABBITMQ_USERNAME", rabbitUser)
    .WithEnvironment("RABBITMQ_PASSWORD", rabbitPassword)
    .WithEnvironment("MINIO_USERNAME", minioUser)
    .WithEnvironment("MINIO_PASSWORD", minioPassword);

builder.AddProject<Ahead_Backend>("Backend")
    .WithReference(rabbitMq)
    .WithEnvironment("RABBITMQ_USERNAME", rabbitUser)
    .WithEnvironment("RABBITMQ_PASSWORD", rabbitPassword)
    .WaitFor(rabbitMq);

builder.Build().Run();