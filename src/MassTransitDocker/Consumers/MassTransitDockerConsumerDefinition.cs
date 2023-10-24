using MassTransit;
using MassTransitDocker.Consumers;

namespace Company.Consumers;

public class MassTransitDockerConsumerDefinition : ConsumerDefinition<MassTransitDockerConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<MassTransitDockerConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
    }
}
