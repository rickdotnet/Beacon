namespace Beacon.Contracts.Models;

public class Healthcheck
{
    public ulong Id { get; set; }
    public bool IsHealthy { get; set; }
    public DateTime SignalTime { get; set; }

    public int ComponentId { get; set; }
    //protected Component? Component { get; set; }
}

public static class HealthCheckExtentions
{
    public static bool IsHealthyWithin(this Healthcheck? healthcheck, TimeSpan withinTime)
        =>
            healthcheck is { IsHealthy: true } 
            && healthcheck.SignalTime.Add(withinTime) > DateTime.UtcNow;
}