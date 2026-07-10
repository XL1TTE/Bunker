using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using FluentValidation;

namespace Bunker.ContentService.Api.Validation;

internal sealed class UpdateHealthCardValidator : AbstractValidator<CardRequest.Put.HealthCard>
{
    public UpdateHealthCardValidator()
    {
        RuleFor(x => x.Health)
            .MinimumLength(4)
            .MaximumLength(64);
    }
}