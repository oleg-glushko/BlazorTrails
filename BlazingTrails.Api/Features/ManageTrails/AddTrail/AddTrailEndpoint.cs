using BlazingTrails.Api.Persistence;
using BlazingTrails.Api.Persistence.Entities;
using BlazingTrails.Shared.Features.ManageTrails.AddTrail;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BlazingTrails.Api.Features.ManageTrails.AddTrail;

public static class AddTrailEndpoint
{
    public static async Task<Results<Ok<int>, ValidationProblem>> Handler(BlazingTrailsContext _database,
        IValidator<AddTrailRequest> validator, AddTrailRequest request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var trail = new Trail
        {
            Name = request.Trail.Name,
            Description = request.Trail.Description,
            Location = request.Trail.Location,
            TimeInMinutes = request.Trail.TimeInMinutes,
            Length = request.Trail.Length,
            Waypoints = request.Trail.Waypoints.Select(wp => new Waypoint
            {
                Latitude = wp.Latitude,
                Longitude = wp.Longitude,
            }).ToList()
        };

        _database.Add(trail);
        await _database.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(trail.Id);
    }
}