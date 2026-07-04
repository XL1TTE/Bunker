using System.Security.Claims;
using Bunker.Api.Common.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Bunker.Api.Common.Middlewares;

public class UserIdentityMiddleware(RequestDelegate next)
{
    public async Task Invoke(
        HttpContext context,
        IUserIdentityContext identityContext,
        ILogger<UserIdentityMiddleware> logger)
    {
        var identity = context.User.Identity;

        if (identity is { IsAuthenticated: true })
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var nickname = context.User.FindFirst("preferred_username")?.Value ?? context.User.FindFirst(ClaimTypes.Name)?.Value;

            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(nickname))
            {
                identityContext.SetUser(userId, nickname, email);
                logger.LogInformation($"[AUTH] User context populated: {identityContext.UserId} {identityContext.Nickname}");
            }
        }

        await next(context);
    }
}
