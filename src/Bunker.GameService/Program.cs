using Bunker.Api.Common;
using Bunker.GameService.Api.Configuration;
using Bunker.GameService.Messaging.Configuration;
using Bunker.GameService.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.IncludeAuthentication();
builder.IncludeOpenApiDocumentation();

builder.ConfigureLogging();
builder.ConfigureSentry();
builder.AddServiceDefaults();

builder.IncludePersistence();
builder.ConfigureWolverine();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.IncludeScalar("Bunker Game Service");
    await app.InitializeDatabaseAsync();
}

app.Run();
