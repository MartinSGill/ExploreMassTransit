using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace MassTransitDocker.Consumers;

[UsedImplicitly]
public class MassTransitDockerConsumer3 : IConsumer<Contracts.MassTransitDocker3>
{
    private readonly ILogger<MassTransitDockerConsumer3> _logger;

    public MassTransitDockerConsumer3(ILogger<MassTransitDockerConsumer3> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<Contracts.MassTransitDocker3> context)
    {
        _logger.LogInformation(
            "{Name} Received {Value} (ConversatonId: {ConversationId}, Initiator: {InitiatorId})",
            GetType().Name,
            context.Message.Value,
            context.ConversationId,
        context.InitiatorId);
        return Task.CompletedTask;
    }
}
