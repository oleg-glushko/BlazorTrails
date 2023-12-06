using BlazingTrails.Api.Persistence;
using BlazingTrails.Api.Persistence.Entities;
using BlazingTrails.Shared.Features.ManageTrails.EditTrail;
using BlazingTrails.Shared.Features.ManageTrails.Shared;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazingTrails.Api.Features.ManageTrails.EditTrail;

public static class EditTrailEndpoint
{
    public static async Task<Results<Ok<bool>, BadRequest<string>, ValidationProblem, UnauthorizedHttpResult>>
        Handler(ClaimsPrincipal user, BlazingTrailsContext _database,IValidator<EditTrailRequest> validator,
                EditTrailRequest request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var trail = await _database.Trails
            .Include(x => x.Waypoints)
            .SingleOrDefaultAsync(x => x.Id == request.Trail.Id, cancellationToken);

        if (trail is null)
            return TypedResults.BadRequest("Trail could not be found");

        if (!trail.Owner.Equals(user.Identity!.Name, StringComparison.OrdinalIgnoreCase)
            && !user.IsInRole("Administrator"))
            return TypedResults.Unauthorized();

        trail.Name = request.Trail.Name;
        trail.Description = request.Trail.Description;
        trail.Location = request.Trail.Location;
        trail.TimeInMinutes = request.Trail.TimeInMinutes;
        trail.Length = request.Trail.Length;
        trail.Waypoints = request.Trail.Waypoints.Select(wp => new Waypoint
        {
            Latitude = wp.Latitude,
            Longitude = wp.Longitude
        }).ToList();

        if (request.Trail.ImageAction == ImageAction.Remove)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Images", trail.Image!);
            System.IO.File.Delete(path);
            trail.Image = null;
        }

        await _database.SaveChangesAsync(cancellationToken);
        
        return TypedResults.Ok(true);
    }
}
