using Ahead.Common;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
// Why we do this? Because gremlin exposes a type called T in that namespace 🤯
using AnonymousTraversalSource = Gremlin.Net.Process.Traversal.AnonymousTraversalSource;
using GraphTraversalSource = Gremlin.Net.Process.Traversal.GraphTraversalSource;

namespace Ahead.Web.Infrastructure;

public interface IAheadGraphDatabase
{
    public Task RunJob(IGremlinJob job);
    public Task<T> RunJob<T>(IGremlinJob<T> job);
}

public class AheadGraphDatabase : IAheadGraphDatabase
{
    private readonly GremlinClient gremlinClient;
    private readonly DriverRemoteConnection remote;

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
        remote = new DriverRemoteConnection(gremlinClient);
    }

    public async Task RunJob(IGremlinJob job) => await job.Run(new GraphContext(gremlinClient, remote));
    public async Task<T> RunJob<T>(IGremlinJob<T> job) => await job.Run(new GraphContext(gremlinClient, remote));

    private class GraphContext(GremlinClient client, DriverRemoteConnection remote) : IGraphContext
    {
        public async Task<IReadOnlyList<TOut>> Run<TIn,TOut>(Func<GraphTraversalSource,Gremlin.Net.Process.Traversal.GraphTraversal<TIn,TOut>> query)
        {
            var traversal = AnonymousTraversalSource.Traversal().WithRemote(remote);
            var tr = query(traversal);
            return (IReadOnlyList<TOut>)await tr.Promise(t => t.ToList());
        }
        
        public async Task Run(string query)
        {
            await client.SubmitAsync(query);
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
    Task<IReadOnlyList<TOut>> Run<TIn,TOut>(Func<GraphTraversalSource,Gremlin.Net.Process.Traversal.GraphTraversal<TIn,TOut>> query);
}