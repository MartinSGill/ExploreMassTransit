using JetBrains.Annotations;
using MassTransit;

namespace MassTransitDocker.Consumers;

[UsedImplicitly]
public class MassTransitDockerConsumer2Definition : ConsumerDefinition<MassTransitDockerConsumer2>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<MassTransitDockerConsumer2> consumerConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
    }
}
