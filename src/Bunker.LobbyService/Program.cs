using Bunker.Api.Common;
using Bunker.Api.Common.Middlewares;
using Bunker.LobbyService.Api.Configuration;
using Bunker.LobbyService.Api.Middlewares;
using Bunker.LobbyService.Endpoints.Configuration;
using Bunker.LobbyService.Messaging.Configuration;
using Bunker.LobbyService.Persistence.Configuration;
using Bunker.LobbyService.Validation.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddHttpContextAccessor();

builder.IncludeIdentityContext();
builder.ConfigureJsonOptions();
builder.IncludeAuthentication();
builder.IncludeOpenApiDocumentation();

builder.IncludeFluentValidation();
builder.IncludePersistence();
builder.IncludeRedis();
builder.ConfigureWolverine();

var app = builder.Build();

app.UseMiddleware<ExceptionToHttpErrorMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserIdentityMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.IncludeScalar("Bunker Lobby Service");
    await app.InitializeDatabaseAsync();
}

app.IncludeLobbyEndpoints();

app.UseHttpsRedirection();
app.Run();
