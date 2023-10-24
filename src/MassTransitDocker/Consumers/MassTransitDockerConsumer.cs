using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace MassTransitDocker.Consumers;

public class MassTransitDockerConsumer : IConsumer<Contracts.MassTransitDocker>
{
    private readonly ILogger<MassTransitDockerConsumer> _logger;

    public MassTransitDockerConsumer(ILogger<MassTransitDockerConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<Contracts.MassTransitDocker> context)
    {
        _logger.LogInformation("Received {Value}", context.Message.Value);
        return Task.CompletedTask;
    }
}
