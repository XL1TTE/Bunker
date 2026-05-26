
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using FluentValidation;

namespace Bunker.ContentService.Api.Validation;

internal sealed class UpdateFactCardValidator : AbstractValidator<CardRequest.Put.FactCard>
{
    public UpdateFactCardValidator()
    {
        RuleFor(x => x.Fact)
            .MinimumLength(4)
            .MaximumLength(64);
    }
}
