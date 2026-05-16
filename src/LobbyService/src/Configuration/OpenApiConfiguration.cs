using Scalar.AspNetCore;
using MicroElements.AspNetCore.OpenApi.FluentValidation;
using Bunker.Infrastructure.Configuration;

namespace LobbyService.Configuration;

internal static class OpenApiConfiguration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder IncludeOpenApiDocumentation()
        {
            builder.Services.AddFluentValidationRulesToOpenApi();

            builder.Services.AddOpenApi(options =>
            {
                options.AddFluentValidationRules();
            });

            return builder;
        }
    }

    extension(WebApplication app)
    {
        public WebApplication IncludeScalar(string title)
        {
            app.MapScalarApiReference("/scalar", options =>
            {
                options.Title = title;
                options.Layout = ScalarLayout.Classic;

                var authority = app.Configuration.GetAuthority();
                var clientId = app.Configuration.GetValue<string>("Identity:ClientId");

                options.AddPreferredSecuritySchemes("oauth2");
                options.AddAuthorizationCodeFlow("oauth2", flow =>
                {
                    flow.ClientId = clientId;
                    flow.ClientSecret = app.Configuration.GetValue<string>("Identity:ClientSecret");

                    flow.AuthorizationUrl = $"{authority}/protocol/openid-connect/auth";
                    flow.TokenUrl = $"{authority}/protocol/openid-connect/token";
                    flow.Pkce = Pkce.Sha256;
                });

                options.AddPasswordFlow("auth", flow =>
                {
                    flow.TokenUrl = $"{authority}/protocol/openid-connect/token";
                    flow.ClientId = clientId;
                    flow.ClientSecret = app.Configuration.GetValue<string>("Identity:ClientSecret");
                });
            });

            return app;
        }
    }
}
