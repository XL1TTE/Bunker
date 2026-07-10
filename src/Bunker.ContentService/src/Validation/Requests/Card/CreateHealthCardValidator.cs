using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using FluentValidation;

namespace Bunker.ContentService.Api.Validation;

internal sealed class CreateHealthCardValidator : AbstractValidator<CardRequest.Post.HealthCard>
{
    public CreateHealthCardValidator()
    {
        RuleFor(x => x.Health)
            .MinimumLength(4)
            .MaximumLength(64);
    }
}