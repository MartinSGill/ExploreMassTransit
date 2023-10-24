using JetBrains.Annotations;
using MassTransit;

namespace MassTransitDocker.Consumers;

[UsedImplicitly]
public class MassTransitDockerConsumerDefinition : ConsumerDefinition<MassTransitDockerConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<MassTransitDockerConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
    }
}
