using Bunker.GameService.Api.Endpoints;

namespace Bunker.GameService.Endpoints.Configuration;

internal static partial class IRouteBuilderExtensions
{
    internal static void IncludeGameEndpoints(this IEndpointRouteBuilder builder)
    {
        var root = builder.MapGroup("/game")
            .WithTags("Game");

        root.MapGet("/{gameId:guid}", GameEndpoints.GetOne);
        root.MapPost("/{gameId:guid}/reveal", GameEndpoints.Reveal);
        root.MapPost("/{gameId:guid}/chat", GameEndpoints.SendMessage);
        root.MapGet("/{gameId:guid}/chat", GameEndpoints.GetMessages);
        root.MapPost("/{gameId:guid}/vote", GameEndpoints.Vote);
    }
}