using FluentValidation;

namespace Bunker.ContentService.Features.PersonalityPresets;

public class CreatePersonalityPresetValidator : AbstractValidator<CreatePersonalityPreset>
{
    public CreatePersonalityPresetValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(4);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10);
    }
}

public class UpdatePersonalityPresetValidator : AbstractValidator<UpdatePersonalityPreset>
{
    public UpdatePersonalityPresetValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MinimumLength(4);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10);
    }
}
