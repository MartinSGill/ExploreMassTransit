using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using MassTransit;
using MassTransitDocker.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Debugging;
using Serilog.Exceptions;

namespace MassTransitDocker.MyHost;

public class MyHostBuilder : HostBuilder, IMyHostBuilder
{
    private readonly Lazy<ILogger> _lazyLog;
    private readonly MyHostBuilderOptions _options;

    private MyHostBuilder(MyHostBuilderOptions options)
    {
        _lazyLog = new Lazy<ILogger>(InitLogger);
        _options = options;
    }

    public bool IsRunningInContainer => new Lazy<bool>(
        () => bool.TryParse(
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
            out var inContainer) && inContainer).Value;

    public ILogger Log => _lazyLog.Value;

    public static IMyHostBuilder Create(string[]? args)
    {
        return Create(args, _ => { });
    }

    public static IMyHostBuilder Create(string[]? args, Action<MyHostBuilderOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var options = new MyHostBuilderOptions();
        configure.Invoke(options);

        var builder = new MyHostBuilder(options);
        builder.ConfigureDefaults(args)
               .UseSerilog(builder.ConfigureSerilog)
               .ConfigureServices(builder.ConfigureMassTransit);
        return builder;
    }


    private void ConfigureMassTransit(HostBuilderContext hostContext, IServiceCollection services)
    {
        Log.Information("Options Root {OptionsRoot}", _options.SectionKey);
        Log.Information("RabbitMq Options Root {OptionsRoot}", RabbitMqOptions.DefaultSection);
        var rabbitMqOptions = hostContext
                              .Configuration
                              .GetSection(_options.SectionKey)
                              .GetSection(RabbitMqOptions.DefaultSection)
                              .Get<RabbitMqOptions>();

        Log.Verbose("RabbitMqOptions {@RabbitMqOptions}", rabbitMqOptions);
        Log.Information("Adding MassTransit");

        services.AddMassTransit(
            x =>
            {
                x.AddDelayedMessageScheduler();

                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(rabbitMqOptions.EndpointPrefix, false));

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
                        Log.Information("Adding RabbitMq");
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

    private void ConfigureSerilog(
        HostBuilderContext context,
        IServiceProvider services,
        LoggerConfiguration configuration)
    {
        Log.Information("Adding Serilog");
        configuration.ReadFrom.Configuration(context.Configuration)
                     .ReadFrom.Services(services)
                     .Enrich.FromLogContext()
                     .Enrich.WithThreadId()
                     .Enrich.WithThreadName()
                     .Enrich.WithExceptionDetails();
    }

    private ILogger InitLogger()
    {
        SelfLog.Enable(Console.Out);
        SelfLog.Enable(msg => Debug.WriteLine(msg));

        return new LoggerConfiguration()
               .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
               .WriteTo.Seq(IsRunningInContainer ? "http://seq:5431" : "http://localhost:5431")
               .CreateBootstrapLogger();
    }
}
