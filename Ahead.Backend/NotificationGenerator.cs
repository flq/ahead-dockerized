using Ahead.Backend.Infrastructure;
using Ahead.Common.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Constants = Ahead.Common.Constants;

namespace Ahead.Backend;

public class NotificationGenerator(BroadcastListener<PagePublishedEvent> eventBroadcast, ILogger<NotificationGenerator> logger) : IHostedService
{
    public Task StartAsync(CancellationToken token)
    {
        logger.LogInformation("Starting notification generator");

        _ = Task.Run(async () =>
        {
            await foreach (var publishedEvent in eventBroadcast.StartListening(Constants.BroadcastExchanges.UserEvents, nameof(NotificationGenerator), token))
                logger.LogInformation("Received page published event for page id {pageId}, will generate relevant notification",
                publishedEvent.PageId);
        }, token);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

