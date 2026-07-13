using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bunker.LobbyService.Persistence;

public interface ILobbyRepository : IRepository<Domain.Lobby, Domain.Lobby.Id>
{
    Task<Domain.Lobby?> TryFindByInviteCodeAsync(InviteCode code, CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(Domain.Lobby.Id key, CancellationToken cancellationToken = default);
}

public partial class LobbyDbContext : ILobbyRepository
{
    public void Add(Domain.Lobby aggregate) => Lobbies.Add(aggregate);

    public async Task<Domain.Lobby?> TryFindAsync(Domain.Lobby.Id key)
        => await Lobbies
            .Include(x => x.Participants)
            .Include(x => x.Packs)
            .FirstOrDefaultAsync(x => x.PublicId == key);

    public async Task<Domain.Lobby?> TryFindByInviteCodeAsync(InviteCode code, CancellationToken cancellationToken = default)
        => await Lobbies
            .Include(x => x.Participants)
            .Include(x => x.Packs)
            .FirstOrDefaultAsync(x => x.InviteCode == code, cancellationToken);

    public async Task<bool> DeleteByIdAsync(Domain.Lobby.Id key, CancellationToken cancellationToken = default)
    {
        var entity = await Lobbies.FirstOrDefaultAsync(x => x.PublicId == key, cancellationToken);
        if (entity is null) return false;
        Lobbies.Remove(entity);
        return true;
    }
}
