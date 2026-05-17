using LobbyService.Features.CreateLobby;

namespace LobbyService.Features.UpdateSettings;

/// <summary>
/// Updated configuration for an existing lobby.
/// </summary>
/// <param name="Capacity">The new maximum participant limit. Must be at least the current number of human players.</param>
/// <param name="Bots">The updated list of AI bots.</param>
/// <param name="Packs">The updated selection of card packs.</param>
/// <param name="IsPublic">The new visibility status of the lobby.</param>
public record UpdateSettingsRequest(
    int Capacity,
    IEnumerable<BotRequest> Bots,
    IEnumerable<string> Packs,
    bool IsPublic);

public abstract record UpdateSettingsResult;
public record UpdateSettingsSuccess : UpdateSettingsResult;
public record UpdateSettingsNotFound : UpdateSettingsResult;
public record UpdateSettingsForbidden : UpdateSettingsResult;
public record UpdateSettingsFailure(string Reason) : UpdateSettingsResult;
public record UpdateSettingsUnauthorized : UpdateSettingsResult;

public static class UpdateSettingsResultFactory
{
    extension (UpdateSettingsResult)
    {
        public static UpdateSettingsResult Success() => new UpdateSettingsSuccess();
        public static UpdateSettingsResult NotFound() => new UpdateSettingsNotFound();
        public static UpdateSettingsResult Forbidden() => new UpdateSettingsForbidden();
        public static UpdateSettingsResult Failure(string reason) => new UpdateSettingsFailure(reason);
        public static UpdateSettingsResult Unauthorized() => new UpdateSettingsUnauthorized();
    }
}
