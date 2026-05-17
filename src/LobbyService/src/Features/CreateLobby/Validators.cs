using FluentValidation;

namespace LobbyService.Features.CreateLobby;

internal class BotRequestValidator : AbstractValidator<BotRequest>
{
    public BotRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PersonalityPreset).NotEmpty();
    }
}

internal class CreateLobbyRequestValidator : AbstractValidator<CreateLobbyRequest>
{
    public CreateLobbyRequestValidator()
    {
        RuleFor(x => x.Capacity).InclusiveBetween(2, 20);
        RuleForEach(x => x.Bots).SetValidator(new BotRequestValidator());
        RuleFor(x => x.Packs).NotEmpty();
    }
}
