using Ahead.Common;
using Ahead.Web.Infrastructure;

namespace Ahead.Web.Endpoints;

public static class BlobStorageRelated
{
    public static void AddBlobStorageRelatedThings(this IEndpointRouteBuilder app)
    {
        app.MapGet("/file/{fileId}", async (string fileId, IBlobStorage storage) =>
            Results.Redirect(await storage.GetPreSignedUrl(Constants.Storage.StaticBucket, fileId)));
    }
}