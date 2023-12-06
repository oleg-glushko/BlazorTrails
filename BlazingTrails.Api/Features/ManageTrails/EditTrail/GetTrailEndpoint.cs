using BlazingTrails.Api.Persistence;
using BlazingTrails.Shared.Features.ManageTrails.EditTrail;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazingTrails.Api.Features.ManageTrails.EditTrail;

public static class GetTrailEndpoint
{
    public static async Task<Results<Ok<GetTrailRequest.Response>, BadRequest<string>, UnauthorizedHttpResult>>
        Handler(ClaimsPrincipal user, BlazingTrailsContext _database, int trailId,
                CancellationToken cancellationToken)
    {
        var trail = await _database.Trails
            .Include(x => x.Waypoints)
            .SingleOrDefaultAsync(x => x.Id == trailId, cancellationToken);

        if (trail is null)
            return TypedResults.BadRequest("Trail could not be found.");

        if (!trail.Owner.Equals(user.Identity?.Name, StringComparison.OrdinalIgnoreCase)
            && !user.IsInRole("Administrator"))
            return TypedResults.Unauthorized();

        var response = new GetTrailRequest.Response(new GetTrailRequest.Trail(
            trail.Id,
            trail.Name,
            trail.Location,
            trail.Image,
            trail.TimeInMinutes,
            trail.Length,
            trail.Description,
            trail.Owner,
            trail.Waypoints.Select(wp =>
                new GetTrailRequest.Waypoint(wp.Latitude, wp.Longitude))));

        return TypedResults.Ok(response);
    }
}
