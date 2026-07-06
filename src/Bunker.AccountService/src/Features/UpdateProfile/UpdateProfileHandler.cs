using Bunker.AccountService.Domain;
using Bunker.AccountService.Features.GetProfile;
using Bunker.AccountService.Messages;
using Bunker.AccountService.Persistence.Repository;
using Wolverine.Attributes;

namespace Bunker.AccountService.Features.UpdateProfile;

[WolverineHandler]
public static class UpdateProfileHandler
{
    public static async IAsyncEnumerable<object> Handle(UpdateProfile command, IUnitOfWork unit)
    {
        var accounts = unit.GetRepository<IAccountRepository>();

        var player = accounts.Find(Account.Id.Create(command.Id));
        if (player is null)
        {
            yield return UpdateProfileResult.NotFound();
            yield break;
        }

        player.UpdateNickname(Nickname.Create(command.Nickname));

        var response = new PlayerProfileResponse(
            player.PublicId.Value,
            player.Nickname.Value,
            player.Stats.TotalGames,
            player.Stats.Wins,
            player.Stats.Losses);

        yield return UpdateProfileResult.Success(response);
        yield return new AccountUpdated(command.Id, command.Nickname, string.Empty);
    }
}