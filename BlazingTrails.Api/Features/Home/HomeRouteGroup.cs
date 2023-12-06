using BlazingTrails.Api.Features.Home.Shared;
using BlazingTrails.Shared.Features.Home.Shared;

namespace BlazingTrails.Api.Features.Home;

public static class HomeRouteGroup
{
    public static void MapHomeEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet(GetTrailsRequest.RouteTemplate, GetTrailsEndpoint.Handler);
    }
}
