namespace PlayerService.Endpoints.Routing;

internal static partial class IRouteBuilderExtensions
{
    internal static void IncludePlayerEndpoints(this IEndpointRouteBuilder builder)
    {
        var root = builder.MapGroup("/player");

        root.MapGet("/me", PlayerEndpoints.GetMyProfile)
            .WithSummary("Get current user profile")
            .WithDescription("Retrieves the persistent profile and aggregate stats for the authenticated user.");

        root.MapGet("/{id}", PlayerEndpoints.GetProfileById)
            .WithSummary("Get player profile by ID")
            .WithDescription("Retrieves the public profile and aggregate stats for any player by their ID.");
    }
}
