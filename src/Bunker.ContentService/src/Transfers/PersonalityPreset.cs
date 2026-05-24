namespace Bunker.ContentService.Transfers;

public abstract partial class Transfer
{
    public readonly record struct PersonalityPreset(Guid Id, string Title, string Description);
}
