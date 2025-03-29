using System.Net.Mime;
using System.Web;
using Ahead.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Ahead.Web.Endpoints;

public static class BlobStorageRelated
{
    public static void AddBlobStorageRelatedThings(this WebApplication app)
    {
        app.MapGet("/upload", ([FromQuery] string? name = null) =>
        Results.Text($"""
                      <html>
                      <body>
                          {(name != null ? $"<h1>{name} successfully uploaded</h1>" : "")}
                          <form action="/upload" method="post" enctype="multipart/form-data">
                              <input type="file" name="file" required>
                              <button type="submit">Upload a File</button>
                          </form>
                      </body>
                      </html>
                      """, MediaTypeNames.Text.Html));

        app.MapPost("/upload", async (IFormFile file, IBlobStorage blobStorage, CancellationToken cancellationToken) =>
        {
            var objectName = await blobStorage.UploadFile("test", file, cancellationToken);

            return Results.Redirect(GenerateUploadUrlAfterUpload(objectName));

            static string GenerateUploadUrlAfterUpload(string objectName)
            {
                return $"/upload?name={HttpUtility.UrlEncode(objectName)}";
            }
        }).DisableAntiforgery();

    }
}