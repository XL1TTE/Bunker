using Bunker.LobbyService.Api.Endpoints.Request;
using FluentValidation;

namespace Bunker.LobbyService.Validation;

internal sealed class CreateLobbyRequestValidator : AbstractValidator<CreateLobby>
{
    public CreateLobbyRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(64);

        RuleFor(x => x.Capacity)
            .GreaterThanOrEqualTo(4);

        RuleFor(x => x.SelectedPackIds)
            .NotNull()
            .Must(ids => ids.All(id => Guid.TryParse(id, out _)))
            .WithMessage("SelectedPackIds must contain only guid strings.");
    }
}

internal sealed class UpdateSettingsRequestValidator : AbstractValidator<UpdateSettingsRequest>
{
    public UpdateSettingsRequestValidator()
    {
        RuleFor(x => x.Capacity)
            .GreaterThanOrEqualTo(4)
            .When(x => x.Capacity is not null);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(64)
            .When(x => x.Name is not null);

        RuleFor(x => x.SelectedPackIds)
            .Must(ids => ids is null || ids.All(id => Guid.TryParse(id, out _)))
            .WithMessage("SelectedPackIds must contain only guid strings.");
    }
}

internal sealed class AddBotRequestValidator : AbstractValidator<AddBotRequest>
{
    public AddBotRequestValidator()
    {
        RuleFor(x => x.PersonalityPresetId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Nickname)
            .NotEmpty()
            .MaximumLength(32);
    }
}

internal sealed class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(500);
    }
}