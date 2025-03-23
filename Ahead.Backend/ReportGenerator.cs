using Ahead.Backend.Infrastructure;
using Ahead.Common.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Constants = Ahead.Common.Constants;

namespace Ahead.Backend;

public class ReportGenerator(QueueListener<ReportRequest> queue, ILogger<ReportGenerator> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting ReportGenerator");

        _ = Task.Run(async () =>
        {
            await foreach (var reportRequest in queue.StartListening(Constants.QueueNames.Basic, cancellationToken))
                logger.LogInformation("Report request of type {reportType} received for date {reportDate}", reportRequest.Type,
                reportRequest.RelevantDate);

        }, cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}