using System.Net.Mime;
using Ahead.Common;
using Ahead.Common.Dto;
using Ahead.Web.Infrastructure;

namespace Ahead.Web.Endpoints;

public static class AppRoutes
{
    public static void MapAppRoutes(this WebApplication app)
    {
        app.MapGet("/", () =>
            Results.Text("""
                         <html>
                            <head><title>Ahead.Web</title></head>
                            <body>
                            <h1>Do something</h1>
                            <ul>
                            <li><a href='/report'>Trigger Report</a></li>
                            <li><a href='/publish'>"Publish" a Page</a></li>
                            <li><a href='/upload'>Upload a File</a></li>
                            </ul>
                            </body>
                         </html>
                         """, MediaTypeNames.Text.Html));
        
        app.AddQueueingAndBroadcasting();
        app.AddBlobStorageRelatedThings();
    }
    
}