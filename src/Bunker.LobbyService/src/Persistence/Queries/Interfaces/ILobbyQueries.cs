using Bunker.LobbyService.Domain;
using Microsoft.EntityFrameworkCore;
using Shared.Monads.Result;

namespace Bunker.LobbyService.Persistence.Queries;

public interface ILobbyQueries
{
    Task<Domain.Lobby?> GetByIdAsync(Domain.Lobby.Id id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Domain.Lobby> Items, int Total)> ListPublicAsync(int limit, int offset, CancellationToken cancellationToken = default);
    Task<Domain.Lobby?> GetByPlayerIdAsync(Domain.AccountId player, CancellationToken cancellationToken = default);
}

public sealed class LobbyQueries(LobbyDbContext db) : ILobbyQueries
{

    public async Task<Domain.Lobby?> GetByIdAsync(Domain.Lobby.Id id, CancellationToken cancellationToken = default)
    {
        return await db.Lobbies
            .Include(x => x.Participants)
            .Include(x => x.Packs)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.PublicId == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Domain.Lobby> Items, int Total)> ListPublicAsync(int limit, int offset, CancellationToken cancellationToken = default)
    {
        var query = db.Lobbies
            .Include(x => x.Participants)
            .Include(x => x.Packs)
            .AsNoTracking()
            .Where(l => l.PrivacyPolicy.IsVisible && l.State != Domain.LobbyState.InGame);

        var total = await query.CountAsync(cancellationToken);
        var lobbies = await query
            .OrderByDescending(l => l.PublicId)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return (lobbies, total);
    }

    public async Task<Domain.Lobby?> GetByPlayerIdAsync(AccountId player, CancellationToken cancellationToken = default)
    {
        return await db.Lobbies
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Participants.OfType<Domain.Player>().Any(p => p.UserId == player), cancellationToken);
    }
}
