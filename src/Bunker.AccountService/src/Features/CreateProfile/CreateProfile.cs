using Bunker.AccountService.Domain;
using Bunker.AccountService.Messages;
using Bunker.AccountService.Persistence.Repository;
using Wolverine.Attributes;

namespace Bunker.AccountService.Features.CreateProfile;

[WolverineHandler]
public static class CreateProfileHandler
{
    public static async IAsyncEnumerable<object> Handle(CreateProfile command, IUnitOfWork unit)
    {
        var player = Account.Create(
            id: Account.Id.Create(command.Id),
            nickname: Nickname.Create(command.Nickname),
            stats: Stats.Create());

        var accounts = unit.GetRepository<IAccountRepository>();

        accounts.Add(player);

        await unit.CommitAsync();
        yield return new AccountUpdated(command.Id.ToString(), command.Nickname, command.Email);
    }
}
