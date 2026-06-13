
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using Bunker.ContentService.Domain;
using FluentValidation;

namespace Bunker.ContentService.Api.Validation;

internal sealed class CreateSexCardValidator : AbstractValidator<CardRequest.Post.SexCard>
{
    public CreateSexCardValidator()
    {
        RuleFor(x => x.Sex)
            .Must(sex => Enum.TryParse<SexCard.SexEnum>(sex, out _))
                .WithMessage("Sex value must be one of the followed [Female, Male].");
    }
}
