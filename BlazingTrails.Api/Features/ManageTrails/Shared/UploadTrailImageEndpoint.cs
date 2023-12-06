using BlazingTrails.Api.Persistence;
using ImageMagick;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazingTrails.Api.Features.ManageTrails.Shared;

public static class UploadTrailImageEndpoint
{
    public static async Task<Results<ContentHttpResult, BadRequest<string>, UnauthorizedHttpResult>> Handler(
        BlazingTrailsContext _database, HttpContext context,
        [FromRoute] int trailId, CancellationToken cancellationToken)
    {
        var trail = await _database.Trails
            .SingleOrDefaultAsync(x => x.Id == trailId, cancellationToken);
        if (trail is null)
            return TypedResults.BadRequest("Trail does not exist.");

        if (!trail.Owner.Equals(context.User.Identity!.Name, StringComparison.OrdinalIgnoreCase)
            && !context.User.IsInRole("Administrator"))
            return TypedResults.Unauthorized();

        var file = context.Request.Form.Files?[0];
        if (file is null || file.Length == 0)
            return TypedResults.BadRequest("No image found.");

        using var image = FormFileToResizedImage(file, 640, 426);
        if (image is null)
            return TypedResults.BadRequest("The uploaded file is not a legitimate image.");

        var filename = $"{Guid.NewGuid()}.jpg";
        var saveLocation = Path.Combine(Directory.GetCurrentDirectory(), "Images", filename);
        await image.WriteAsync(saveLocation, MagickFormat.Jpg, cancellationToken);

        if (!string.IsNullOrWhiteSpace(trail.Image))
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Images", trail.Image);
            File.Delete(path);
        }

        trail.Image = filename;
        await _database.SaveChangesAsync(cancellationToken);

        return TypedResults.Text(trail.Image);
    }

    private static MagickImage? FormFileToResizedImage(IFormFile imageFile, int width, int height, bool padding = true)
    {
        MagickImage image;
        try
        {
            image = new MagickImage(imageFile.OpenReadStream());
        }
        catch (MagickException)
        {
            return default;
        }

        var size = new MagickGeometry(width, height);

        if (image.Width > width || image.Height > height)
            image.Resize(size);

        if (!padding || image.Width == width && image.Height == height)
            return image;

        var background = new MagickImage(MagickColors.White, width, height);
        background.Composite(image, Gravity.Center, CompositeOperator.Over);
        image.Dispose();
        return background;
    }
}
