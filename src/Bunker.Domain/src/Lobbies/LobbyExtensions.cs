using System.Collections.Immutable;
using Bunker.Domain.Bots;
using Shared.Monads.Result;
using static Bunker.Domain.Identity.User;

namespace Bunker.Domain.Lobbies;

public static class LobbyExtensions
{
    extension (Lobby lobby)
    {
        public bool AllReady => lobby.Players.All(p => p.Status is Ready);

        public Result<Lobby, LobbyErrors.AddPlayerError> AddPlayer(Player player)
        {
            if (lobby.Players.Any(p => Equals(p.Id, player.Id)))
                return Result<Lobby, LobbyErrors.AddPlayerError>.Failure(new LobbyErrors.PlayerAlreadyInLobby());

            if (lobby.Players.Count + lobby.Bots.Count >= lobby.Capacity)
                return Result<Lobby, LobbyErrors.AddPlayerError>.Failure(new LobbyErrors.LobbyIsFull());

            lobby.Players.Add(player);
            return Result<Lobby, LobbyErrors.AddPlayerError>.Success(lobby);
        }

        public Result<Lobby, LobbyErrors.RemovePlayerError> RemovePlayer(UserId playerId)
        {
            var player = lobby.Players.FirstOrDefault(p => Equals(p.Id, playerId));
            if (player == null)
                return Result<Lobby, LobbyErrors.RemovePlayerError>.Failure(new LobbyErrors.PlayerNotFound());

            lobby.Players.Remove(player);
            
            // Handle Host Migration
            if (player.Role is Host && lobby.Players.Any())
            {
                lobby.Players[0] = lobby.Players[0] with { Role = Role.Host };
            }

            return Result<Lobby, LobbyErrors.RemovePlayerError>.Success(lobby);
        }

        public Result<Lobby, LobbyErrors.ToggleReadyError> ToggleReady(UserId playerId)
        {
            var player = lobby.Players.FirstOrDefault(p => p.Id == playerId);
            if (player == null)
                return Result<Lobby, LobbyErrors.ToggleReadyError>.Failure(new LobbyErrors.PlayerNotInLobby());

            player = player with { Status = Status.Ready };

            return Result<Lobby, LobbyErrors.ToggleReadyError>.Success(lobby);
        }

        public Lobby SetVisibility(bool isPublic)
        {
            lobby.Visibility = isPublic ? LobbyVisibility.Public : LobbyVisibility.Private;
            return lobby;
        }
    }
}
