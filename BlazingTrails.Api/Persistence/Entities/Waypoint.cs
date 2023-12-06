namespace BlazingTrails.Api.Persistence.Entities;

public class Waypoint
{
    public int Id { get; set; }
    public int TrailId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public Trail Trail { get; set; } = null!;
}
