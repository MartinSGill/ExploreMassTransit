namespace MassTransitDocker.Options;

public class RabbitMqOptions
{
    public const string DefaultSection = "RabbitMq";
    public string Host { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string EndpointPrefix { get; set; } = string.Empty;
}
