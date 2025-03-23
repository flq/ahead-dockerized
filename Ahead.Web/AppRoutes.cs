using System.Net.Mime;
using Ahead.Common;
using Ahead.Common.Dto;
using Ahead.Web.Infrastructure;

namespace Ahead.Web;

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
                            </ul>
                            </body>
                         </html>
                         """, MediaTypeNames.Text.Html));
        
        app.MapGet("/report", async (QueueSender sender, TimeProvider timeProvider) =>
        {
            await sender.Send(Constants.QueueNames.Basic, 
                new ReportRequest
                {
                    Type = "onboarding",
                    RelevantDate = timeProvider.GetUtcNow().Date
                });
            return Results.Text("Sent report request!");
        });
        
        app.MapGet("/publish", async (BroadcastSender sender, Random random) =>
        {
            await sender.Send(Constants.BroadcastExchanges.UserEvents, 
            new PagePublishedEvent(random.Next(1, 1000).ToString()));
            return Results.Text("Page has been published!");
        });
    }
    
}