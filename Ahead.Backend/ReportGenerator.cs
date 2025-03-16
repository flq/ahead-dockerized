using Ahead.Common.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Constants = Ahead.Common.Constants;

namespace Ahead.Backend;

public class ReportGenerator(QueueListener<ReportRequest> queue, ILogger<ReportGenerator> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken token)
    {
        await foreach (var reportRequest in queue.StartListening(Constants.QueueNames.Basic, token))
            logger.LogInformation("Report request of type {reportType} received for date {reportDate}", reportRequest.Type, reportRequest.RelevantDate);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}