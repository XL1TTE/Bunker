using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using FluentValidation;

namespace Bunker.ContentService.Api.Validation;

internal sealed class UpdateLuggageCardValidator : AbstractValidator<CardRequest.Put.LuggageCard>
{
    public UpdateLuggageCardValidator()
    {
        RuleFor(x => x.Luggage)
            .MinimumLength(4)
            .MaximumLength(64);
    }
}