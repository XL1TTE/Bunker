namespace Bunker.AccountService.Endpoints;

internal static partial class IRouteBuilderExtensions
{
    internal static void IncludeAccountEndpoints(this IEndpointRouteBuilder builder)
    {
        var root = builder.MapGroup("/account");

        root.MapGet("/me", AccountEndpoints.GetMyProfile)
            .WithSummary("Get current user profile")
            .WithDescription("Retrieves the persistent profile and aggregate stats for the authenticated user.");

        root.MapGet("/{id}", AccountEndpoints.GetProfileById)
            .WithSummary("Get account profile by ID")
            .WithDescription("Retrieves the public profile and aggregate stats for any account by their ID.");
    }
}
