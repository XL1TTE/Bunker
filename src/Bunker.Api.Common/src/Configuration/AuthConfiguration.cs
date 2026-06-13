using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Bunker.Api.Common;

public static class AuthConfiguration
{
    public static string? GetAuthority(this IConfiguration configuration)
    {
        var authBaseUrl = configuration.GetConnectionString("auth")
                          ?? configuration.GetValue<string>("AUTH_HTTPS")
                          ?? configuration.GetValue<string>("AUTH_HTTP");

        var realm = configuration.GetValue<string>("Identity:Realm") ?? "master";
        return authBaseUrl is null ? null : $"{authBaseUrl.TrimEnd('/')}/realms/{realm}";
    }

    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder IncludeAuthentication()
        {
            var authority = builder.Configuration.GetAuthority();
            var clientId = builder.Configuration.GetValue<string>("Identity:ClientId");
            var audience = builder.Configuration.GetValue<string>("Identity:Audience");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                   .AddJwtBearer(options =>
                   {
                       options.Authority = authority;
                       options.Audience = audience;
                       options.RequireHttpsMetadata = false;
                       options.TokenValidationParameters = new TokenValidationParameters
                       {
                           ValidateIssuerSigningKey = true,
                           ValidateIssuer = true,
                           ValidateAudience = true,
                           ValidIssuer = authority,
                           ValidAudience = clientId,
                       };
                       
                       // For debugging 401 issues
                       options.Events = new JwtBearerEvents
                       {
                           OnAuthenticationFailed = context =>
                           {
                               Console.WriteLine($"Auth failed: {context.Exception.Message}");
                               return Task.CompletedTask;
                           },
                           OnForbidden = context =>
                           {
                               Console.WriteLine($"Forbidden: {context}");
                               return Task.CompletedTask;
                           }
                       };
                   });

            builder.Services.AddAuthorization();

            return builder;
        }
    }
}
