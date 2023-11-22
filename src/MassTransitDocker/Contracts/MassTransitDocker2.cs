using System;
using MassTransit;

namespace Contracts;

public record MassTransitDocker2
{
    public string Value { get; init; }
}
