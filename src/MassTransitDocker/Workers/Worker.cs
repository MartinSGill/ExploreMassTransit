using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MassTransitDocker.Workers;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IBus _bus;

    public Worker(ILogger<Worker> logger, IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = new Contracts.MassTransitDocker() { Value = "Hello MassTransitDocker" };
            var correlationId = Guid.NewGuid();
            _logger.LogInformation(
                "Worker running at: {Time}, with CorrId: {CorrelationId}",
                DateTimeOffset.Now,
                correlationId);

            // Conversation vs Correlation: https://stackoverflow.com/a/55300111/270178
            // Can tweak the context here so that I can pass the correlationId
            // allowing me to return it to the caller (if this were via an API)
            await _bus.Publish(
                message,
                context => { context.CorrelationId = correlationId; },
                stoppingToken);

            await Task.Delay(10_000, stoppingToken);
        }
    }
}
