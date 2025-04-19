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
    private readonly GremlinClient gremlinClient;
    
    public AheadGraphDatabase(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        var connectionString = configuration.GetConnectionString(GraphDbConnectionString.Name);
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Graph connection string not found");
        }
        var connection = new GraphDbConnectionString(connectionString);
        var server = new GremlinServer(
            connection.Host, 
            connection.Port, 
            connection.UseSsl, 
            connection.Username, 
            connection.Password);
        gremlinClient = new GremlinClient(server, loggerFactory: loggerFactory);
    }

    public async Task RunJob(IGremlinJob job) => await job.Run(new GraphContext(gremlinClient));
    public async Task<T> RunJob<T>(IGremlinJob<T> job) => await job.Run(new GraphContext(gremlinClient));

    private class GraphContext(GremlinClient client) : IGraphContext
    {
        
        public async Task Run(string query)
        {
            await client.SubmitAsync(query);
            var result = await client.SubmitAsync<object>("g.V().count()");
        }
    }
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
    Task Run(string query);
}