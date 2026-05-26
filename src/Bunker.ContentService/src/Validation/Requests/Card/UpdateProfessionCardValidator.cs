
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using FluentValidation;

namespace Bunker.ContentService.Api.Validation;

internal sealed class UpdateProfessionCardValidator : AbstractValidator<CardRequest.Put.ProfessionCard>
{
    public UpdateProfessionCardValidator()
    {
        RuleFor(x => x.Profession)
            .MinimumLength(4)
            .MaximumLength(64);
    }
}
