using Ahead.Common;
using Gremlin.Net.Driver;

namespace Ahead.Web.Infrastructure;

public interface IAheadGraphDatabase
{
    public Task RunJob(IGremlinJob job);
    public Task<T> RunJob<T>(IGremlinJob<T> job);
}

public class AheadGraphDatabase : IAheadGraphDatabase
{
    private readonly GremlinServer server;
    
    public AheadGraphDatabase(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(GraphDbConnectionString.Name);
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Graph connection string not found");
        }
        var connection = new GraphDbConnectionString(connectionString);
        server = new GremlinServer(
            connection.Host, 
            connection.Port, 
            connection.UseSsl, 
            connection.Username, 
            connection.Password);
    }
    
    public GremlinClient CreateClient() => new(server);
    
    public Task RunJob(IGremlinJob job) => throw new NotImplementedException();
    public Task<T> RunJob<T>(IGremlinJob<T> job) => throw new NotImplementedException();
}

public interface IGremlinJob
{
    Task Run(IGraphContext graphContext);
}

public interface IGremlinJob<T>
{
    Task<T> Run(IGraphContext graphContext);
}

public interface IGraphContext
{
    
}