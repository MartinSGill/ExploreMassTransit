using MassTransitDocker.MyHost;
using MassTransitDocker.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = MyHostBuilder.Create(args);
builder.ConfigureServices(
    (_, services) =>
    {
        services.AddHostedService<Worker>();
    });

await builder.Build().RunAsync();
