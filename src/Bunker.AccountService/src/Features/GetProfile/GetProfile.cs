using Wolverine.Attributes;
using Bunker.AccountService.Domain;
using Bunker.AccountService.Persistence.Repository;

namespace Bunker.AccountService.Features.GetProfile;

[WolverineHandler]
public static class GetProfileHandler
{
    public static async Task<GetProfileResult> Handle(GetProfile query, IUnitOfWork unit)
    {
        var accounts = unit.GetRepository<IAccountRepository>();

        var player = accounts.Find(Account.Id.Create(query.Id));

        if (player is null) 
            return GetProfileResult.NotFound();

        var response = new PlayerProfileResponse(
            player.PublicId.Value,
            player.Nickname.Value,
            player.Stats.TotalGames,
            player.Stats.Wins,
            player.Stats.Losses);

        return GetProfileResult.Success(response);
    }
}
