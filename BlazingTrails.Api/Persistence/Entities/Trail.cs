namespace BlazingTrails.Api.Persistence.Entities;

public class Trail
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Image { get; set; }
    public string Location { get; set; } = string.Empty;
    public int TimeInMinutes { get; set; }
    public int Length { get; set; }
    public string Owner { get; set; } = string.Empty;

    public ICollection<Waypoint> Waypoints { get; set; } = new List<Waypoint>();
}
