namespace AppHost;

public static class InfrastructureDependencies
{
    public static IResourceBuilder<ContainerResource>? AddBlobStorage(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ParameterResource> userName,
        IResourceBuilder<ParameterResource> password,
        StartupOptions startupOptions)
    {
        if (startupOptions.HasFlag(StartupOptions.Storage))
        {
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
        return null;
    }

    public static IResourceBuilder<RabbitMQServerResource>? AddMessaging(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ParameterResource> userName,
        IResourceBuilder<ParameterResource> password,
        StartupOptions startupOptions)
    {
        if (startupOptions.HasFlag(StartupOptions.Messaging))
        {
            return builder
                .AddRabbitMQ("messaging", userName, password)
                .WithManagementPlugin();
        }
        return null;
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