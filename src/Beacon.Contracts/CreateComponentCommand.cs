using Apollo.Messaging.Abstractions;

namespace Beacon.Contracts;

public record CreateComponentCommand : ICommand
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string[] Environments { get; set; } = Array.Empty<string>();
    public string[] Tags { get; set; } = Array.Empty<string>();
    public string[] Owners { get; set; } = Array.Empty<string>();
    public string[] Links { get; set; } = Array.Empty<string>();
}

public record ComponentCreatedEvent : IEvent
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string[] Environments { get; set; } = Array.Empty<string>();
    public string[] Tags { get; set; } = Array.Empty<string>();
    public string[] Owners { get; set; } = Array.Empty<string>();
    public string[] Links { get; set; } = Array.Empty<string>();
}