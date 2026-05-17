using LobbyService.Features.CreateLobby;

namespace LobbyService.Features.BrowseLobbies;

/// <summary>
/// A summary of a public lobby visible in the browser.
/// </summary>
/// <param name="Id">The unique identifier of the lobby.</param>
/// <param name="HostNickname">The nickname of the player who owns the lobby.</param>
/// <param name="Capacity">The maximum number of participants allowed.</param>
/// <param name="CurrentParticipants">The number of human players currently in the lobby.</param>
/// <param name="BotsCount">The number of AI bots configured for this lobby.</param>
/// <param name="Packs">The names or identifiers of the card packs selected for this lobby.</param>
public record LobbySummaryResponse(
    Guid Id,
    string HostNickname,
    int Capacity,
    int CurrentParticipants,
    int BotsCount,
    IEnumerable<string> Packs);

public abstract record BrowseLobbiesResult;
public record BrowseLobbiesSuccess(IEnumerable<LobbySummaryResponse> Lobbies) : BrowseLobbiesResult;

public static class BrowseLobbiesResultFactory
{
    extension (BrowseLobbiesResult)
    {
        public static BrowseLobbiesResult Success(IEnumerable<LobbySummaryResponse> lobbies) => new BrowseLobbiesSuccess(lobbies);
    }
}
