using JetBrains.Annotations;
using MassTransit;

namespace MassTransitDocker.Consumers;

[UsedImplicitly]
public class MassTransitDockerConsumer3Definition : ConsumerDefinition<MassTransitDockerConsumer3>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<MassTransitDockerConsumer3> consumerConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
    }
}
