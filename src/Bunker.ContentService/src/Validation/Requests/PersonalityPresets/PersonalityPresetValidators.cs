using FluentValidation;
using Bunker.ContentService.Api.PersonalityPresets.Endpoints.Requests;

namespace Bunker.ContentService.Validation.Requests.PersonalityPresets;

public class CreatePersonalityPresetValidator : AbstractValidator<PersonalityPresetRequest.Post.Create>
{
    public CreatePersonalityPresetValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(4);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10);
    }
}

public class UpdatePersonalityPresetValidator : AbstractValidator<PersonalityPresetRequest.Put.Update>
{
    public UpdatePersonalityPresetValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(4);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10);
    }
}
