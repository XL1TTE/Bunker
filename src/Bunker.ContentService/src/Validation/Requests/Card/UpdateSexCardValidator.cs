
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using Bunker.ContentService.Domain;
using FluentValidation;

namespace Bunker.ContentService.Api.Validation;

internal sealed class UpdateSexCardValidator : AbstractValidator<CardRequest.Put.SexCard>
{
    public UpdateSexCardValidator()
    {
        RuleFor(x => x.Sex).Must(sex => Enum.TryParse<SexCard.SexEnum>(sex, out _));
    }
}
