
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using FluentValidation;

namespace Bunker.ContentService.Api.Validation;

internal sealed class CreateAgeCardValidator : AbstractValidator<CardRequest.Post.AgeCard>
{
    public CreateAgeCardValidator()
    {
        RuleFor(x => x.Age)
            .GreaterThan(5)
            .LessThan(100);
    }
}
