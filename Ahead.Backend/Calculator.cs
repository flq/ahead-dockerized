using Ahead.Common.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Constants = Ahead.Common.Constants;

namespace Ahead.Backend;

public class Calculator(QueueListener<CalculationRequest> queue, ILogger<Calculator> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken token)
    {
        await foreach (var calculationRequest in queue.StartListening(Constants.QueueNames.Basic, token))
            logger.LogInformation("Calculation received: {calculation}", calculationRequest.Calculation);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}