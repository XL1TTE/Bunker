using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace Bunker.ContentService.Endpoints.PersonalityPresets;

internal static partial class IRouteBuilderExtensions
{
    internal static void IncludePersonalityPresetEndpoints(this IEndpointRouteBuilder builder)
    {
        var root = builder.MapGroup("/content/bots");

        root.MapPost("/", PersonalityPresetEndpoints.Create)
            .WithSummary("Create personality preset")
            .WithDescription("Creates a new personality preset for bots.");

        root.MapPut("/{id:guid}", PersonalityPresetEndpoints.Update)
            .WithSummary("Update personality preset")
            .WithDescription("Updates an existing personality preset.");

        root.MapDelete("/{id:guid}", PersonalityPresetEndpoints.Delete)
            .WithSummary("Delete personality preset")
            .WithDescription("Deletes a personality preset.");

        root.MapGet("/{id:guid}", PersonalityPresetEndpoints.GetById)
            .WithSummary("Get personality preset by ID")
            .WithDescription("Retrieves a personality preset by its ID.");

        root.MapGet("/", PersonalityPresetEndpoints.GetAll)
            .WithSummary("Get all personality presets")
            .WithDescription("Retrieves all personality presets.");
    }
}
