using FluentValidation;
using LobbyService.Features.CreateLobby;

namespace LobbyService.Features.UpdateSettings;

internal class UpdateSettingsRequestValidator : AbstractValidator<UpdateSettingsRequest>
{
    public UpdateSettingsRequestValidator()
    {
        RuleFor(x => x.Capacity).InclusiveBetween(2, 20);
        RuleForEach(x => x.Bots).SetValidator(new BotRequestValidator());
        RuleFor(x => x.Packs).NotEmpty();
    }
}
