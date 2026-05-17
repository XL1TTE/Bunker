using Bunker.Infrastructure.Configuration;
using Bunker.Infrastructure.Middlewares;
using LobbyService.Configuration;
using LobbyService.Endpoints.Routing;
using LobbyService.Middlewares;

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
