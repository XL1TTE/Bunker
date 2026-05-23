using Bunker.ContentService.Api.Cards.Endpoints;

namespace Bunker.ContentService.Api.Endpoints.Cards;

internal static partial class IRouteBuilderExtensions
{
    internal static void IncludeCardEndpoints(this IEndpointRouteBuilder builder)
    {
        var root = builder.MapGroup("/content/cards");

        root.MapPost("/profession", ProfessionCardEndpoints.CreateProfession)
            .WithSummary("Create profession card")
            .WithDescription("Creates a new profession card that defines a character's job or background.");
            
        root.MapPut("/profession/{id:guid}", ProfessionCardEndpoints.UpdateProfession)
            .WithSummary("Update profession card")
            .WithDescription("Updates an existing profession card's details.");
            
        root.MapGet("/profession", ProfessionCardEndpoints.GetProfessionCards)
            .WithSummary("Get all profession cards")
            .WithDescription("Retrieves the full library of profession cards.");

        root.MapPost("/hobbies", HobbiesCardEndpoints.CreateHobbies)
            .WithSummary("Create hobbies card")
            .WithDescription("Creates a new hobbies card representing character interests.");
            
        root.MapPut("/hobbies/{id:guid}", HobbiesCardEndpoints.UpdateHobbies)
            .WithSummary("Update hobbies card")
            .WithDescription("Updates an existing hobbies card.");
            
        root.MapGet("/hobbies", HobbiesCardEndpoints.GetHobbiesCards)
            .WithSummary("Get all hobbies cards")
            .WithDescription("Retrieves the full library of hobbies cards.");

        root.MapPost("/age", AgeCardEndpoints.CreateAge)
            .WithSummary("Create age card")
            .WithDescription("Creates a new age card [0-254].");
            
        root.MapPut("/age/{id:guid}", AgeCardEndpoints.UpdateAge)
            .WithSummary("Update age card")
            .WithDescription("Updates an existing age card.");
            
        root.MapGet("/age", AgeCardEndpoints.GetAgeCards)
            .WithSummary("Get all age cards")
            .WithDescription("Retrieves the full library of age cards.");

        root.MapPost("/sex", SexCardEndpoints.CreateSex)
            .WithSummary("Create sex card")
            .WithDescription("Creates a new sex card.");
            
        root.MapPut("/sex/{id:guid}", SexCardEndpoints.UpdateSex)
            .WithSummary("Update sex card")
            .WithDescription("Updates an existing sex card.");
            
        root.MapGet("/sex", SexCardEndpoints.GetSexCards)
            .WithSummary("Get all sex cards")
            .WithDescription("Retrieves the full library of sex cards.");

        root.MapPost("/fact", FactCardEndpoints.CreateFact)
            .WithSummary("Create fact card")
            .WithDescription("Creates a new fact card with character-specific trivia.");
            
        root.MapPut("/fact/{id:guid}", FactCardEndpoints.UpdateFact)
            .WithSummary("Update fact card")
            .WithDescription("Updates an existing fact card.");
            
        root.MapGet("/fact", FactCardEndpoints.GetFactCards)
            .WithSummary("Get all fact cards")
            .WithDescription("Retrieves the full library of fact cards.");

        root.MapDelete("/{id:guid}", CardEndpoints.Delete)
            .WithSummary("Delete card")
            .WithDescription("Permanently removes a card from the content library by its ID.");
            
        root.MapGet("/{id:guid}", CardEndpoints.GetById)
            .WithSummary("Get card by ID")
            .WithDescription("Retrieves the details of any card type by its public ID.");
    }
}
