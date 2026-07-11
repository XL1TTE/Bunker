using Bunker.LobbyService.Api.Endpoints;

namespace Bunker.LobbyService.Endpoints.Configuration;

internal static partial class IRouteBuilderExtensions
{
    internal static void IncludeLobbyEndpoints(this IEndpointRouteBuilder builder)
    {
        var root = builder.MapGroup("/lobbies")
            .WithTags("Lobbies");

        root.MapPost("/", LobbyEndpoints.Create);

        root.MapPost("/{inviteCode}/join", LobbyEndpoints.JoinByInviteCode);
        root.MapPost("/{lobbyId:guid}/join", LobbyEndpoints.JoinByPassword);

        root.MapPost("/{lobbyId:guid}/leave", LobbyEndpoints.Leave);
        root.MapGet("/{lobbyId:guid}", LobbyEndpoints.GetOne);
        root.MapGet("/{lobbyId:guid}/invite-code", LobbyEndpoints.GetInviteCode);
        root.MapGet("/", LobbyEndpoints.List);
        root.MapPatch("/{lobbyId:guid}/settings", LobbyEndpoints.UpdateSettings);

        root.MapPost("/{lobbyId:guid}/bots", LobbyEndpoints.AddBot);
        root.MapDelete("/{lobbyId:guid}/bots/{participantId:guid}", LobbyEndpoints.RemoveBot);
        root.MapDelete("/{lobbyId:guid}/participants/{participantId:guid}", LobbyEndpoints.KickParticipant);

        root.MapPost("/{lobbyId:guid}/ready", LobbyEndpoints.ToggleReadiness);
        root.MapPost("/{lobbyId:guid}/start", LobbyEndpoints.Start);
        root.MapPost("/{lobbyId:guid}/messages", LobbyEndpoints.SendMessage);
    }
}