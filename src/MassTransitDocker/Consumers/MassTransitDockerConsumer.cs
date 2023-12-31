using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace MassTransitDocker.Consumers;

[UsedImplicitly]
public class MassTransitDockerConsumer : IConsumer<Contracts.MassTransitDocker>
{
    private readonly ILogger<MassTransitDockerConsumer> _logger;
    private readonly IBus _bus;

    public MassTransitDockerConsumer(ILogger<MassTransitDockerConsumer> logger, IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    public Task Consume(ConsumeContext<Contracts.MassTransitDocker> context)
    {
        _logger.LogInformation(
            "{Name} Received {Value} (ConversatonId: {ConversationId}, Initiator: {InitiatorId})",
            GetType().Name,
            context.Message.Value,
            context.ConversationId,
            context.InitiatorId);
        _bus.Publish(new Contracts.MassTransitDocker2 { Value = context.Message.Value });
        return Task.CompletedTask;
    }
}
