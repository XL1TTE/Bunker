using Bunker.Domain.Identity;
using PlayerService.Persistence;
using Wolverine.Attributes;

namespace PlayerService.Features.CreateProfile;

[WolverineHandler]
public static class CreateProfileHandler
{
    public static async Task Handle(CreateProfile command, PlayerDbContext db)
    {
        var player = User.Restore(
            id: User.Id.Restore(command.Id),
            nickname: Nickname.Restore(command.Nickname),
            stats: Stats.Create());

        db.Players.Add(player);
        await db.SaveChangesAsync();
    }
}
