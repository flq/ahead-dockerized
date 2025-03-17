using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("mqUsername", secret: true);
var password = builder.AddParameter("mqPassword", secret: true);

var rabbitMq = builder
    .AddRabbitMQ("messaging", username, password)
    .WithManagementPlugin();

builder.AddProject<Ahead_Web>("Frontend")
    .WithReference(rabbitMq)
    .WithEnvironment("RABBITMQ_USERNAME", username)
    .WithEnvironment("RABBITMQ_PASSWORD", password)
    .WaitFor(rabbitMq);

builder.AddProject<Ahead_Backend>("Backend")
    .WithReference(rabbitMq)
    .WithEnvironment("RABBITMQ_USERNAME", username)
    .WithEnvironment("RABBITMQ_PASSWORD", password)
    .WaitFor(rabbitMq);

builder.Build().Run();