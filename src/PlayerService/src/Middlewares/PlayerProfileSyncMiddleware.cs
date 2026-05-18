using Bunker.Infrastructure.Identity;
using PlayerService.Features.CreateProfile;
using PlayerService.Persistence;
using Wolverine;
using Microsoft.EntityFrameworkCore;
using Bunker.Domain.Identity;

namespace PlayerService.Middlewares;

public class PlayerProfileSyncMiddleware(RequestDelegate next)
{
    public async Task Invoke(
        HttpContext context,
        IUserIdentityContext identityContext,
        PlayerDbContext db,
        IMessageBus bus,
        ILogger<PlayerProfileSyncMiddleware> logger)
    {
        if (identityContext.IsAuthenticated)
        {
            var userId = identityContext.UserId!.Value;
            var exists = await db.Players.AnyAsync(x => x.PublicId == User.Id.Restore(userId));

            if (!exists)
            {
                logger.LogInformation("[SYNC] Profile not found for user {UserId}. Creating...", userId);
                
                // Blocking invoke to ensure profile exists for the current request
                await bus.InvokeAsync(new CreateProfile(userId, identityContext.Nickname!));
            }
        }

        await next(context);
    }
}
