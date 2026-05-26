
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using FluentValidation;

namespace Bunker.ContentService.Api.Validation;

internal sealed class UpdateHobbiesCardValidator : AbstractValidator<CardRequest.Put.HobbiesCard>
{
    public UpdateHobbiesCardValidator()
    {
        RuleFor(x => x.Hobbies)
            .MinimumLength(4)
            .MaximumLength(64);
    }
}
