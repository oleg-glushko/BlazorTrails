using BlazingTrails.Api.Persistence;
using BlazingTrails.Shared.Features.Home.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BlazingTrails.Api.Features.Home.Shared;

public static class GetTrailsEndpoint
{
    public static async Task<Ok<GetTrailsRequest.Response>> Handler
        (BlazingTrailsContext _database, CancellationToken cancellationToken)
    {
        var trails = await _database.Trails.AsNoTracking()
            .Include(x => x.Waypoints)
            .ToListAsync(cancellationToken);
        var response = new GetTrailsRequest.Response(trails.Select(trail => new GetTrailsRequest.Trail(
            trail.Id,
            trail.Name,
            trail.Image,
            trail.Location,
            trail.TimeInMinutes,
            trail.Length,
            trail.Description,
            trail.Owner,
            trail.Waypoints.Select(wp =>
                new GetTrailsRequest.Waypoint(wp.Latitude, wp.Longitude)).ToList())));

        return TypedResults.Ok(response);
    }
}
