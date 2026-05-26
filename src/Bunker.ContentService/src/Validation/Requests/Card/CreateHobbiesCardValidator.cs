
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using FluentValidation;

namespace Bunker.ContentService.Api.Validation;

internal sealed class CreateHobbiesCardValidator : AbstractValidator<CardRequest.Post.HobbiesCard>
{
    public CreateHobbiesCardValidator()
    {
        RuleFor(x => x.Hobbies)
            .MinimumLength(4)
            .MaximumLength(64);
    }
}
