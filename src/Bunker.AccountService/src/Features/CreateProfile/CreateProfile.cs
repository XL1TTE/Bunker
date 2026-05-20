using Bunker.AccountService.Domain;
using Bunker.AccountService.Persistence;
using Wolverine.Attributes;

namespace Bunker.AccountService.Features.CreateProfile;

[WolverineHandler]
public static class CreateProfileHandler
{
    public static async Task Handle(CreateProfile command, AccountDbContext db)
    {
        var player = Account.Restore(
            id: Account.Id.Restore(command.Id),
            nickname: Nickname.Restore(command.Nickname),
            stats: Stats.Create());

        db.Accounts.Add(player);
        await db.SaveChangesAsync();
    }
}
