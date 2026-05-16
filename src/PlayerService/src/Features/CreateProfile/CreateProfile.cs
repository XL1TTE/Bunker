using Bunker.Domain.Identity;
using PlayerService.Persistence;
using Wolverine.Attributes;

namespace PlayerService.Features.CreateProfile;

[WolverineHandler]
public static class CreateProfileHandler
{
    public static async Task Handle(CreateProfile command, PlayerDbContext db)
    {
        var player = Player.Create(
            PlayerId.Restore(command.Id), 
            Nickname.Restore(command.Nickname));
            
        db.Players.Add(player);
        await db.SaveChangesAsync();
    }
}
