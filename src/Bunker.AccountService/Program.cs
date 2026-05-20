using Bunker.AccountService.Api.Configuration;
using Bunker.AccountService.Api.Middlewares;
using Bunker.AccountService.Endpoints;
using Bunker.Api.Common;
using Bunker.Api.Common.Middlewares;

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
builder.ConfigureWolverine();

var app = builder.Build();

app.UseMiddleware<ExceptionToHttpErrorMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Populate Identity Context
app.UseMiddleware<UserIdentityMiddleware>();

// Sync Profile on first login
app.UseMiddleware<PlayerProfileSyncMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.IncludeScalar("Bunker Player API");
    await app.InitializeDatabaseAsync();
}

// Endpoint Registration
app.IncludeAccountEndpoints();

app.UseHttpsRedirection();
app.Run();
