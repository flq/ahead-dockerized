using Ahead.Common;
using Ahead.Common.Dto;
using Ahead.Web.Infrastructure;

namespace Ahead.Web.Endpoints;

public static class QueueingAndBroadcasting
{
    public static void AddQueueingAndBroadcasting(this IEndpointRouteBuilder app)
    {
        app.MapGet("/report", async (QueueSender sender, TimeProvider timeProvider) =>
        {
            await sender.Send(
                Constants.QueueNames.Basic,
                new ReportRequest
                {
                    Type = "onboarding",
                    RelevantDate = timeProvider.GetUtcNow().Date
                });

            return Results.Extensions.BackToHomeWithMessage("Report sent!");
        });

        app.MapGet("/publish", async (BroadcastSender sender, Random random) =>
        {
            await sender.Send(
                Constants.BroadcastExchanges.UserEvents,
                new PagePublishedEvent(random.Next(1, 1000).ToString()));
            return Results.Extensions.BackToHomeWithMessage("Page has been published!");
        });
    }
}