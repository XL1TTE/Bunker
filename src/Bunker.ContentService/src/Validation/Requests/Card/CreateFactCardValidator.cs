
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using FluentValidation;

namespace Bunker.ContentService.Api.Validation;

internal sealed class CreateFactCardValidator : AbstractValidator<CardRequest.Post.FactCard>
{
    public CreateFactCardValidator()
    {
        RuleFor(x => x.Fact)
            .MinimumLength(4)
            .MaximumLength(64);
    }
}
