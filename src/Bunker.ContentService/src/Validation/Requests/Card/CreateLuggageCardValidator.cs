using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using FluentValidation;

namespace Bunker.ContentService.Api.Validation;

internal sealed class CreateLuggageCardValidator : AbstractValidator<CardRequest.Post.LuggageCard>
{
    public CreateLuggageCardValidator()
    {
        RuleFor(x => x.Luggage)
            .MinimumLength(4)
            .MaximumLength(64);
    }
}