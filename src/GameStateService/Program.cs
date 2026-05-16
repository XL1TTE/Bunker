using Bunker.Infrastructure.Configuration;
using GameStateService.Configuration;
using GameStateService.Middlewares;

var builder = WebApplication.CreateBuilder(args);

#region Core Logging & Context

builder.Services.AddLogging();
builder.Services.AddHttpContextAccessor();

#endregion


#region Configuration

    builder.ConfigureJsonOptions();
    builder.IncludeFluentValidation();
    builder.IncludeAuthentication();
    builder.IncludePersistence();
    builder.IncludeRedis();
    builder.ConfigureWolverine();
    builder.IncludeOpenApiDocumentation();
    
#endregion

var app = builder.Build();

#region Middlewares

app.UseMiddleware<ExceptionToHttpErrorMiddleware>();

#endregion

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.IncludeScalar("Bunker Game State API");
    
    await app.InitializeDatabaseAsync();
}

#region Endpoints


#endregion

app.UseHttpsRedirection();
app.Run();
