using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace Bunker.ContentService.Endpoints.BunkerCards;

internal static partial class IRouteBuilderExtensions
{
    internal static void IncludeBunkerCardEndpoints(this IEndpointRouteBuilder builder)
    {
        var root = builder.MapGroup("/content/bunker-cards");

        root.MapPost("/", BunkerCardEndpoints.Create)
            .WithSummary("Create bunker card")
            .WithDescription("Creates a new canned bunker card used as AI-generation fallback content.");

        root.MapPut("/{id:guid}", BunkerCardEndpoints.Update)
            .WithSummary("Update bunker card")
            .WithDescription("Updates an existing canned bunker card.");

        root.MapDelete("/{id:guid}", BunkerCardEndpoints.Delete)
            .WithSummary("Delete bunker card")
            .WithDescription("Deletes a canned bunker card.");

        root.MapGet("/{id:guid}", BunkerCardEndpoints.GetById)
            .WithSummary("Get bunker card by ID")
            .WithDescription("Retrieves a canned bunker card by its ID.");

        root.MapGet("/", BunkerCardEndpoints.GetAll)
            .WithSummary("Get all bunker cards")
            .WithDescription("Retrieves all canned bunker cards.");
    }
}