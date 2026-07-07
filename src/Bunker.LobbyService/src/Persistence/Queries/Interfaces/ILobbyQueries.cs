using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Monads.Result;

namespace Bunker.LobbyService.Persistence.Queries;

public interface ILobbyQueries
{
    Task<Result<Domain.Lobby, string>> GetByInviteCodeAsync(string inviteCode, CancellationToken cancellationToken = default);
    Task<Domain.Lobby?> GetByIdAsync(Domain.Lobby.Id id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Domain.Lobby> Items, int Total)> ListPublicAsync(int limit, int offset, CancellationToken cancellationToken = default);
    Task<Domain.Lobby?> GetByHostIdAsync(Domain.AccountId Host, CancellationToken cancellationToken = default);
}

public sealed class LobbyQueries(LobbyDbContext db) : ILobbyQueries
{
    public async Task<Result<Domain.Lobby, string>> GetByInviteCodeAsync(string inviteCode, CancellationToken cancellationToken = default)
    {
        var normalized = inviteCode.Trim().ToUpperInvariant();
        var lobby = await db.Lobbies
            .Include(x => x.Participants)
            .Include(x => x.Packs)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.InviteCode == normalized, cancellationToken);

        if (lobby is null)
            return Result<Domain.Lobby, string>.Failure("Lobby not found.");

        return Result<Domain.Lobby, string>.Success(lobby.ToDomain());
    }

    public async Task<Domain.Lobby?> GetByIdAsync(Domain.Lobby.Id id, CancellationToken cancellationToken = default)
    {
        var lobby = await db.Lobbies
            .Include(x => x.Participants)
            .Include(x => x.Packs)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.PublicId == id.Value, cancellationToken);

        return lobby?.ToDomain();
    }

    public async Task<(IReadOnlyList<Domain.Lobby> Items, int Total)> ListPublicAsync(int limit, int offset, CancellationToken cancellationToken = default)
    {
        var query = db.Lobbies
            .Include(x => x.Participants)
            .Include(x => x.Packs)
            .AsNoTracking()
            .Where(l => l.PrivacyPolicy.IsVisible && l.Status != "InGame");

        var total = await query.CountAsync(cancellationToken);
        var lobbies = await query
            .OrderByDescending(l => l.PublicId)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return (lobbies.Select(l => l.ToDomain()).ToList(), total);
    }

    public async Task<Domain.Lobby?> GetByHostIdAsync(AccountId Host, CancellationToken cancellationToken = default)
    {
        return db.Lobbies.FirstOrDefault(l => l.Participants.OfType<Entities.PlayerParticipant>().Any(p => p.UserId == Host.Value && p.Role == "Host"))?.ToDomain();
    }
}
