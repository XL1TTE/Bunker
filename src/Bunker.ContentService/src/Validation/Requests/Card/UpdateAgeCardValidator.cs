
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using FluentValidation;

namespace Bunker.ContentService.Api.Validation;

internal sealed class UpdateAgeCardValidator : AbstractValidator<CardRequest.Put.AgeCard>
{
    public UpdateAgeCardValidator()
    {
        RuleFor(x => x.Age)
            .GreaterThan(5)
            .LessThan(100);
    }
}
