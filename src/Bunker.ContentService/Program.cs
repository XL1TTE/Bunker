using Bunker.Api.Common;
using Bunker.Api.Common.Middlewares;
using Bunker.ContentService.Api.Configuration;
using Bunker.ContentService.Api.Endpoints.Cards;
using Bunker.ContentService.Api.Middlewares;
using Bunker.ContentService.Endpoints.CardPacks;
using Bunker.ContentService.Endpoints.PersonalityPresets;
using Bunker.ContentService.Messaging.Configuration;
using Bunker.ContentService.Persistence;
using Bunker.ContentService.Validation.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureOpenTelemetry();

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
    app.IncludeScalar("Bunker Content Service API");
    await app.InitializeDatabaseAsync();
}

app.IncludeCardEndpoints();
app.IncludeCardPackEndpoints();
app.IncludePersonalityPresetEndpoints();

app.UseHttpsRedirection();
app.Run();
