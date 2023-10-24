using System;
using System.Reflection;
using System.Threading.Tasks;
using MassTransit;
using MassTransitDocker.Options;
using MassTransitDocker.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;

namespace MassTransitDocker;

public class Program
{
    private static bool? _isRunningInContainer;

    private static bool IsRunningInContainer =>
        _isRunningInContainer ??= bool.TryParse(
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
            out var inContainer) && inContainer;

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                   .UseSerilog(ConfigureSerilog)
                   .ConfigureServices(ConfigureMassTransit)
                   .ConfigureServices(ConfigureWorkers);
    }

    private static void ConfigureWorkers(HostBuilderContext context, IServiceCollection services)
    {
        services.AddHostedService<Worker>();
    }

    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync();
    }

    private static void ConfigureMassTransit(HostBuilderContext hostContext, IServiceCollection services)
    {
        var rabbitMqOptions = hostContext
                              .Configuration
                              .GetSection(nameof(MassTransitDocker))
                              .GetSection(RabbitMqOptions.DefaultSection)
                              .Get<RabbitMqOptions>();

        services.AddMassTransit(
            x =>
            {
                x.AddDelayedMessageScheduler();

                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(prefix: rabbitMqOptions.EndpointPrefix, includeNamespace: false));

                // By default, sagas are in-memory, but should be changed to a durable
                // saga repository.
                x.SetInMemorySagaRepositoryProvider();

                var entryAssembly = Assembly.GetEntryAssembly();

                x.AddConsumers(entryAssembly);
                x.AddSagaStateMachines(entryAssembly);
                x.AddSagas(entryAssembly);
                x.AddActivities(entryAssembly);

                x.UsingRabbitMq(
                    (context, cfg) =>
                    {
                        cfg.Host(
                            rabbitMqOptions.Host,
                            host =>
                            {
                                host.Password(rabbitMqOptions.Password);
                                host.Username(rabbitMqOptions.Username);
                            });

                        cfg.UseDelayedMessageScheduler();

                        // Call LAST.
                        // Registers all detected consumers
                        cfg.ConfigureEndpoints(context);
                    });
            });
    }

    private static void ConfigureSerilog(
        HostBuilderContext context,
        IServiceProvider services,
        LoggerConfiguration configuration)
    {
        configuration.ReadFrom.Configuration(context.Configuration)
                     .ReadFrom.Services(services)
                     .Enrich.FromLogContext()
                     .Enrich.WithThreadId()
                     .Enrich.WithThreadName()
                     .Enrich.WithExceptionDetails();
    }
}
