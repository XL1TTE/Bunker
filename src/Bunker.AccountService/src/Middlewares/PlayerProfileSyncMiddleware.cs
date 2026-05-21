using Wolverine;
using Microsoft.EntityFrameworkCore;
using Bunker.Api.Common.Identity;
using Bunker.AccountService.Persistence;
using Bunker.AccountService.Domain;
using Bunker.AccountService.Features.CreateProfile;

namespace Bunker.AccountService.Api.Middlewares;

public class PlayerProfileSyncMiddleware(RequestDelegate next)
{
    public async Task Invoke(
        HttpContext context,
        IUserIdentityContext identityContext,
        AccountDbContext db,
        IMessageBus bus,
        ILogger<PlayerProfileSyncMiddleware> logger)
    {
        if (identityContext.IsAuthenticated)
        {
            var userId = identityContext.UserId!.Value;
            var exists = await db.Accounts.AnyAsync(x => x.PublicId == Account.Id.Create(userId));

            if (!exists)
            {
                logger.LogInformation("[SYNC] Profile not found for user {UserId}. Creating...", userId);

                // Blocking invoke to ensure profile exists for the current request
                await bus.InvokeAsync(new CreateProfile(userId, identityContext.Nickname!, identityContext.Email!));
            }
        }

        await next(context);
    }
}
