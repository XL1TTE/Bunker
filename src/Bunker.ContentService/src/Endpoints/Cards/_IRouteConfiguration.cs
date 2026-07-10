using Bunker.ContentService.Api.Cards.Endpoints;

namespace Bunker.ContentService.Api.Endpoints.Cards;

internal static partial class IRouteBuilderExtensions
{
    internal static void IncludeCardEndpoints(this IEndpointRouteBuilder builder)
    {
        var root = builder.MapGroup("/content/cards");

        root.MapPost("/profession", ProfessionCardEndpoints.CreateProfessionCard)
            .WithSummary("Create profession card")
            .WithDescription("Creates a new profession card that defines a character's job or background.");

        root.MapPut("/profession/{id:guid}", ProfessionCardEndpoints.UpdateProfessionCard)
            .WithSummary("Update profession card")
            .WithDescription("Updates an existing profession card's details.");
            
        root.MapGet("/profession", ProfessionCardEndpoints.GetProfessionCards)
            .WithSummary("Get all profession cards")
            .WithDescription("Retrieves the full library of profession cards.");

        root.MapPost("/hobbies", HobbiesCardEndpoints.CreateHobbiesCard)
            .WithSummary("Create hobbies card")
            .WithDescription("Creates a new hobbies card representing character interests.");

        root.MapPut("/hobbies/{id:guid}", HobbiesCardEndpoints.UpdateHobbiesCard)
            .WithSummary("Update hobbies card")
            .WithDescription("Updates an existing hobbies card.");
            
        root.MapGet("/hobbies", HobbiesCardEndpoints.GetHobbiesCards)
            .WithSummary("Get all hobbies cards")
            .WithDescription("Retrieves the full library of hobbies cards.");

        root.MapPost("/age", AgeCardEndpoints.CreateAgeCard)
            .WithSummary("Create age card")
            .WithDescription("Creates a new age card [0-254].");

        root.MapPut("/age/{id:guid}", AgeCardEndpoints.UpdateAgeCard)
            .WithSummary("Update age card")
            .WithDescription("Updates an existing age card.");
            
        root.MapGet("/age", AgeCardEndpoints.GetAgeCards)
            .WithSummary("Get all age cards")
            .WithDescription("Retrieves the full library of age cards.");

        root.MapPost("/sex", SexCardEndpoints.CreateSexCard)
            .WithSummary("Create sex card")
            .WithDescription("Creates a new sex card.");

        root.MapPut("/sex/{id:guid}", SexCardEndpoints.UpdateSexCard)
            .WithSummary("Update sex card")
            .WithDescription("Updates an existing sex card.");
            
        root.MapGet("/sex", SexCardEndpoints.GetSexCards)
            .WithSummary("Get all sex cards")
            .WithDescription("Retrieves the full library of sex cards.");

        root.MapPost("/fact", FactCardEndpoints.CreateFactCard)
            .WithSummary("Create fact card")
            .WithDescription("Creates a new fact card with character-specific trivia.");

        root.MapPut("/fact/{id:guid}", FactCardEndpoints.UpdateFactCard)
            .WithSummary("Update fact card")
            .WithDescription("Updates an existing fact card.");
            
        root.MapGet("/fact", FactCardEndpoints.GetFactCards)
            .WithSummary("Get all fact cards")
            .WithDescription("Retrieves the full library of fact cards.");

        root.MapPost("/health", HealthCardEndpoints.CreateHealthCard)
            .WithSummary("Create health card")
            .WithDescription("Creates a new health card describing a character's physical and mental state.");

        root.MapPut("/health/{id:guid}", HealthCardEndpoints.UpdateHealthCard)
            .WithSummary("Update health card")
            .WithDescription("Updates an existing health card.");

        root.MapGet("/health", HealthCardEndpoints.GetHealthCards)
            .WithSummary("Get all health cards")
            .WithDescription("Retrieves the full library of health cards.");

        root.MapPost("/luggage", LuggageCardEndpoints.CreateLuggageCard)
            .WithSummary("Create luggage card")
            .WithDescription("Creates a new luggage card describing the items a character carries.");

        root.MapPut("/luggage/{id:guid}", LuggageCardEndpoints.UpdateLuggageCard)
            .WithSummary("Update luggage card")
            .WithDescription("Updates an existing luggage card.");

        root.MapGet("/luggage", LuggageCardEndpoints.GetLuggageCards)
            .WithSummary("Get all luggage cards")
            .WithDescription("Retrieves the full library of luggage cards.");

        root.MapDelete("/{id:guid}", CardEndpoints.Delete)
            .WithSummary("Delete card")
            .WithDescription("Permanently removes a card from the content library by its ID.");
            
        root.MapGet("/{id:guid}", CardEndpoints.GetById)
            .WithSummary("Get card by ID")
            .WithDescription("Retrieves the details of any card type by its public ID.");
    }
}
