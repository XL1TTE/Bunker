using Bunker.LobbyService.Api.Endpoints;

namespace Bunker.LobbyService.Endpoints.Configuration;

internal static partial class IRouteBuilderExtensions
{
    internal static void IncludeLobbyEndpoints(this IEndpointRouteBuilder builder)
    {
        var root = builder.MapGroup("/lobbies")
            .WithTags("Lobbies");

        root.MapPost("/", LobbyEndpoints.Create);
    }
}
