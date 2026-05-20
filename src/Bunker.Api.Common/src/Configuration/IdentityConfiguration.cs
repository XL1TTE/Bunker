using Bunker.Api.Common.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bunker.Api.Common;

public static class IdentityConfiguration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder IncludeIdentityContext()
        {
            builder.Services.AddScoped<IUserIdentityContext, UserIdentityContext>();
            return builder;
        }
    }
}
