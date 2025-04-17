using System.Text;

namespace AppHost;

public static class InfrastructureDependencies
{
    public static IResourceBuilder<ContainerResource>? AddBlobStorage(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ParameterResource> userName,
        IResourceBuilder<ParameterResource> password,
        StartupOptions startupOptions)
    {
        if (!startupOptions.HasFlag(StartupOptions.Storage))
            return null;
        return builder
            .AddContainer("minio", "minio/minio")
            .WithContainerName("ahead_blobstorage")
            .WithEnvironment("MINIO_ROOT_USER", userName)
            .WithEnvironment("MINIO_ROOT_PASSWORD", password)
            .WithEndpoint(9000, 9000, "http", "minioserver")
            .WithEndpoint(9090, 9090, "http", "minioconsole")
            .WithBindMount("../data/minio", "/data")
            .WithArgs("server", "/data", "--console-address", ":9090");
    }
    
    public static IResourceBuilder<ContainerResource>? AddGraphDatabase(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ParameterResource> arcadeRootPassword,
        StartupOptions startupOptions)
    {
        if (!startupOptions.HasFlag(StartupOptions.GraphDatabase))
            return null;
        StringBuilder javaOptions = new();
        javaOptions.Append($"-Darcadedb.server.rootPassword={arcadeRootPassword.Resource.Value} ");
        javaOptions.Append("-Darcadedb.server.plugins=GremlinServer:com.arcadedb.server.gremlin.GremlinServerPlugin ");
        javaOptions.Append("-Darcadedb.server.databaseDirectory=/data");
            
        return builder
            .AddContainer("arcadedb", "arcadedata/arcadedb")
            .WithContainerName("ahead_graphdb")
            .WithEnvironment("JAVA_OPTS", javaOptions.ToString())
            .WithEndpoint(2480, 2480, "http", "dashboard")
            .WithEndpoint(2424, 2424, name: "db")
            .WithEndpoint(8182, 8182, name: "gremlin")
            .WithBindMount("../data/arcadedb", "/data");
    }

    public static IResourceBuilder<RabbitMQServerResource>? AddMessaging(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ParameterResource> userName,
        IResourceBuilder<ParameterResource> password,
        StartupOptions startupOptions)
    {
        if (!startupOptions.HasFlag(StartupOptions.Messaging))
            return null;
        return builder
            .AddRabbitMQ("messaging", userName, password)
            .WithManagementPlugin();
    }

    public static IResourceBuilder<ProjectResource> ReferenceAndWaitForMessagingIfAvailable(
        this IResourceBuilder<ProjectResource> project,
        IResourceBuilder<RabbitMQServerResource>? messagingResource,
        IResourceBuilder<ParameterResource> userName,
        IResourceBuilder<ParameterResource> password)
    {
        if (messagingResource == null) 
            return project;
        return project.WithReference(messagingResource)
            .WithEnvironment("RABBITMQ_USERNAME", userName)
            .WithEnvironment("RABBITMQ_PASSWORD", password)
            .WaitFor(messagingResource);
    }
}

[Flags]
public enum StartupOptions
{
    None = 0,
    Backend = 1 << 0,
    Storage = 1 << 1,
    Messaging = 1 << 2,
    GraphDatabase = 1 << 3,
    All = Backend | Storage | Messaging | GraphDatabase
}