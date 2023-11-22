using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace MassTransitDocker.Consumers;

[UsedImplicitly]
public class MassTransitDockerConsumer2 : IConsumer<Contracts.MassTransitDocker2>
{
    private readonly ILogger<MassTransitDockerConsumer2> _logger;
    private readonly IBus _bus;

    public MassTransitDockerConsumer2(ILogger<MassTransitDockerConsumer2> logger, IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    public Task Consume(ConsumeContext<Contracts.MassTransitDocker2> context)
    {
        _logger.LogInformation(
            "{Name} Received {Value} (ConversatonId: {ConversationId}, Initiator: {InitiatorId})",
            GetType().Name,
            context.Message.Value,
            context.ConversationId,
        context.InitiatorId);
        _bus.Publish(new Contracts.MassTransitDocker3 { Value = context.Message.Value });
        return Task.CompletedTask;
    }
}
