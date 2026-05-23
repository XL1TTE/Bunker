namespace Bunker.ContentService.Endpoints.CardPacks;

internal static partial class IRouteBuilderExtensions
{
    internal static void IncludeCardPackEndpoints(this IEndpointRouteBuilder builder)
    {
        var root = builder.MapGroup("/content/packs");

        root.MapPost("/", CardPackEndpoints.Create)
            .WithSummary("Create card pack")
            .WithDescription("Creates a new curated collection of cards (professions, facts, etc.) that can be selected for games.");
            
        root.MapPut("/{id:guid}", CardPackEndpoints.Update)
            .WithSummary("Update card pack")
            .WithDescription("Updates the title, description, or card contents of an existing pack.");
            
        root.MapDelete("/{id:guid}", CardPackEndpoints.Delete)
            .WithSummary("Delete card pack")
            .WithDescription("Permanently removes a card pack from the library.");
            
        root.MapGet("/{id:guid}", CardPackEndpoints.GetById)
            .WithSummary("Get card pack by ID")
            .WithDescription("Retrieves the full details and card list of a specific pack.");
            
        root.MapGet("/", CardPackEndpoints.GetAll)
            .WithSummary("Get all card packs")
            .WithDescription("Retrieves the full catalog of available card packs.");

        root.MapPost("/{id:guid}/cards/{cardId:guid}", CardPackEndpoints.AddCard)
            .WithSummary("Add card to pack")
            .WithDescription("Links an existing card to a specific card pack.");
            
        root.MapDelete("/{id:guid}/cards/{cardId:guid}", CardPackEndpoints.RemoveCard)
            .WithSummary("Remove card from pack")
            .WithDescription("Unlinks a card from a pack without deleting the card itself.");
    }
}
