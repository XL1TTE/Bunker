using LobbyService.Features.CreateLobby;
using LobbyService.Features.BrowseLobbies;
using LobbyService.Features.JoinLobby;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LobbyService.Endpoints.Routing;

internal static partial class IRouteBuilderExtensions
{
    internal static void IncludeLobbyEndpoints(this IEndpointRouteBuilder builder)
    {
        var root = builder.MapGroup("/lobbies")
            .WithTags("Lobbies");

        root.MapPost("/", LobbyEndpoints.CreateLobby)
            .WithSummary("Create a new lobby")
            .WithDescription("Initializes a new game lobby with specified capacity, bots, and card packs. The caller becomes the Host.");

        root.MapGet("/", LobbyEndpoints.BrowseLobbies)
            .WithSummary("Browse public lobbies")
            .WithDescription("Retrieves a list of all publicly visible lobbies that are currently in the recruitment phase.");

        root.MapPost("/join", LobbyEndpoints.JoinLobby)
            .WithSummary("Join a lobby via code")
            .WithDescription("Adds the authenticated player to a lobby using a unique 6-character invite code.");

        root.MapPost("/{id:guid}/ready", LobbyEndpoints.ToggleReady)
            .WithSummary("Toggle readiness")
            .WithDescription("Switches the ready status of the current player. All human players must be ready to start the game.");

        root.MapDelete("/{id:guid}/leave", LobbyEndpoints.LeaveLobby)
            .WithSummary("Leave the lobby")
            .WithDescription("Removes the current player from the lobby. If the host leaves, the lobby is disbanded.");

        root.MapPatch("/{id:guid}", LobbyEndpoints.UpdateSettings)
            .WithSummary("Update lobby settings")
            .WithDescription("Allows the host to modify lobby parameters like capacity and bot configurations.");

        root.MapPost("/{id:guid}/start", LobbyEndpoints.StartGame)
            .WithSummary("Start the game")
            .WithDescription("Initiates the transition from lobby to active game. Requires all players to be ready.");
    }
}
