using Bunker.LobbyService.Persistence.Abstractions;
using Bunker.LobbyService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bunker.LobbyService.Persistence;

public interface ILobbyRepository : IRepository<Domain.Lobby, Domain.Lobby.Id>
{
    Task<bool> UpdateAsync(Domain.Lobby aggregate, CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(Domain.Lobby.Id key, CancellationToken cancellationToken = default);
}

public partial class LobbyDbContext : ILobbyRepository
{
    public void Add(Domain.Lobby aggregate) => Lobbies.Add(aggregate.ToEntity());

    public async Task<Domain.Lobby?> TryFindAsync(Domain.Lobby.Id key)
        => (await Lobbies
            .Include(x => x.Participants)
            .Include(x => x.Packs)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PublicId == key.Value))
            ?.ToDomain();

    public async Task<bool> UpdateAsync(Domain.Lobby aggregate, CancellationToken cancellationToken = default)
    {
        var origin = await Lobbies
            .Include(x => x.Participants)
            .Include(x => x.Packs)
            .FirstOrDefaultAsync(x => x.PublicId == aggregate.PublicId.Value, cancellationToken);

        if (origin is null) return false;

        origin.ApplyUpdate(aggregate);
        return true;
    }

    public async Task<bool> DeleteByIdAsync(Domain.Lobby.Id key, CancellationToken cancellationToken = default)
    {
        var entity = await Lobbies.FirstOrDefaultAsync(x => x.PublicId == key.Value, cancellationToken);
        if (entity is null) return false;
        Lobbies.Remove(entity);
        return true;
    }
}