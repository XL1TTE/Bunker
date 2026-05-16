using Bunker.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bunker.Infrastructure.Configuration;

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
