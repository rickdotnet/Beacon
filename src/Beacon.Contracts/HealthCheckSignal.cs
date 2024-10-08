using Apollo.Abstractions;

namespace Beacon.Contracts;

public record HealthCheckSignaledEvent : IEvent
{
    public Guid ComponentId { get; init; } = Guid.Empty;
    public string? ComponentName { get; init; } = string.Empty;
    public bool IsHealthy { get; init; }
    public string? Message { get; init; }
    public SubCheck[] SubChecks { get; init; } = Array.Empty<SubCheck>();
};
public record SubCheck
{
    public bool IsHealthy { get; init; }
    public string? Message { get; init; }
};