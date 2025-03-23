using Ahead.Backend.Infrastructure;
using Ahead.Common.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Constants = Ahead.Common.Constants;

namespace Ahead.Backend;

public class SearchIndexUpdater(BroadcastListener<PagePublishedEvent> eventBroadcast, ILogger<SearchIndexUpdater> logger) : IHostedService
{
    public Task StartAsync(CancellationToken token)
    {
        logger.LogInformation("Starting Search index updater");
        _ = Task.Run(async () =>
        {
            await foreach (var publishedEvent in eventBroadcast.StartListening(Constants.BroadcastExchanges.UserEvents, nameof(SearchIndexUpdater), token))
                logger.LogInformation("Received page published event for page id {pageId}, will update the search index",
                publishedEvent.PageId);
        }, token);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}