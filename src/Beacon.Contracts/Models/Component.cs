namespace Beacon.Contracts.Models;

public class Component
{
    public int Id { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string[] Environments { get; set; } = Array.Empty<string>();
    public string[] Tags { get; set; } = Array.Empty<string>();
    public string[] Owners { get; set; } = Array.Empty<string>();
    public string[] Links { get; set; } = Array.Empty<string>();
}