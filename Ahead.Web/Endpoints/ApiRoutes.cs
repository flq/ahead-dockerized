namespace Ahead.Web.Endpoints;

public static class ApiRoutes
{
    public static void MapApiRoutes(this WebApplication app)
    {
        var api = app.MapGroup("api");
        api.AddQueueingAndBroadcasting();
        api.AddBlobStorageRelatedThings();
    }
}