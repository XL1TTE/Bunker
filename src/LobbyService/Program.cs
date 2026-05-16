using Bunker.Infrastructure.Configuration;
using Bunker.Infrastructure.Middlewares;
using LobbyService.Configuration;
using LobbyService.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddHttpContextAccessor();

// Infrastructure (Shared)
builder.IncludeIdentityContext();
builder.ConfigureJsonOptions();
builder.IncludeAuthentication();
builder.IncludeOpenApiDocumentation();

// Service Specific
builder.IncludeFluentValidation();
builder.IncludePersistence();
builder.IncludeRedis();
builder.ConfigureWolverine();

var app = builder.Build();

app.UseMiddleware<ExceptionToHttpErrorMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Populate Identity Context
app.UseMiddleware<UserIdentityMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.IncludeScalar("Bunker Lobby API");
    await app.InitializeDatabaseAsync();
}

app.UseHttpsRedirection();
app.Run();
