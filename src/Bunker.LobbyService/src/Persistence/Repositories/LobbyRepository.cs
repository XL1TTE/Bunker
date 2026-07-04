using Bunker.LobbyService.Persistence.Abstractions;
using Bunker.LobbyService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bunker.LobbyService.Persistence;

public interface ILobbyRepository : IRepository<Domain.Lobby, Domain.Lobby.Id>{}

public partial class LobbyDbContext : ILobbyRepository
{
    public void Add(Domain.Lobby aggregate) => Lobbies.Add(aggregate.ToEntity());

    public void Delete(Domain.Lobby aggregate) => Lobbies.Remove(aggregate.ToEntity());

    public async Task<Domain.Lobby?> TryFindAsync(Domain.Lobby.Id key)
        => (await Lobbies.FirstOrDefaultAsync(x => x.PublicId == key.Value))?.ToDomain();


    public bool Update(Domain.Lobby aggregate)
    {
        var origin = Lobbies.FirstOrDefault(x => x.PublicId == aggregate.PublicId.Value);
        if (origin is null) return false;

        origin?.ApplyUpdate(aggregate);
        return true;
    }
}
