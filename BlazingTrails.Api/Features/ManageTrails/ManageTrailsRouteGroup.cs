using BlazingTrails.Api.Features.ManageTrails.AddTrail;
using BlazingTrails.Api.Features.ManageTrails.EditTrail;
using BlazingTrails.Api.Features.ManageTrails.Shared;
using BlazingTrails.Shared.Features.ManageTrails.AddTrail;
using BlazingTrails.Shared.Features.ManageTrails.EditTrail;
using BlazingTrails.Shared.Features.ManageTrails.Shared;

namespace BlazingTrails.Api.Features.ManageTrails;

public static class ManageTrailsRouteGroup
{
    public static void MapManageTrailEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet(GetTrailRequest.RouteTemplate, GetTrailEndpoint.Handler)
            .RequireAuthorization();
        routeBuilder.MapPost(AddTrailRequest.RouteTemplate, AddTrailEndpoint.Handler)
            .RequireAuthorization();
        routeBuilder.MapPut(EditTrailRequest.RouteTemplate, EditTrailEndpoint.Handler)
            .RequireAuthorization();

        routeBuilder.MapPost(UploadTrailImageRequest.RouteTemplate, UploadTrailImageEndpoint.Handler)
            .RequireAuthorization();
    }
}
