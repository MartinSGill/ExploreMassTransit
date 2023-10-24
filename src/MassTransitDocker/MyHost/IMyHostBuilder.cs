using Microsoft.Extensions.Hosting;
using Serilog;

namespace MassTransitDocker.MyHost;

public interface IMyHostBuilder : IHostBuilder
{
    /// <summary>
    /// True if running in container.
    /// </summary>
    bool IsRunningInContainer { get; }

    /// <summary>
    /// The initial "bootstrap" logger is able to log errors during start-up. It's completely replaced by the
    /// logger configured in `UseSerilog()` below, once configuration and dependency-injection have both been
    /// set up successfully.
    /// </summary>
    ILogger Log { get; }
}