using Bunker.Api.Common;
using Bunker.Api.Common.Middlewares;
using Bunker.GameService.Api.Configuration;
using Bunker.GameService.Api.Middlewares;
using Bunker.GameService.Configuration;
using Bunker.GameService.Endpoints.Configuration;
using Bunker.GameService.Hubs;
using Bunker.GameService.Messaging.Configuration;
using Bunker.GameService.Persistence;
using Bunker.GameService.Validation.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR();

builder.Services.Configure<GameTimingOptions>(builder.Configuration.GetSection("Game"));

builder.ConfigureLogging();
builder.IncludeIdentityContext();
builder.ConfigureJsonOptions();
builder.IncludeAuthentication();
builder.IncludeOpenApiDocumentation();

builder.IncludeFluentValidation();

builder.ConfigureSentry();
builder.AddServiceDefaults();

builder.IncludePersistence();
builder.ConfigureWolverine();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionToHttpErrorMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserIdentityMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.IncludeScalar("Bunker Game Service");
    await app.InitializeDatabaseAsync();
}

app.IncludeGameEndpoints();

app.MapHub<GameHub>("/hubs/game");

app.Run();