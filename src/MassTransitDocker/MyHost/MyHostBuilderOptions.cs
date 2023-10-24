using System.Reflection;

namespace MassTransitDocker.MyHost;

public class MyHostBuilderOptions
{
    public string SectionKey { get; set; } = Assembly.GetCallingAssembly().GetName().Name!;
}