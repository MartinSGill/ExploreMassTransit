using System;
using MassTransit;

namespace Contracts;

public record MassTransitDocker
{
    public string Value { get; init; }
}
