
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using FluentValidation;

namespace Bunker.ContentService.Api.Validation;

internal sealed class CreateProfessionCardValidator : AbstractValidator<CardRequest.Post.ProfessionCard>
{
    public CreateProfessionCardValidator()
    {
        RuleFor(x => x.Profession)
            .MinimumLength(4)
            .MaximumLength(64);
    }
}
