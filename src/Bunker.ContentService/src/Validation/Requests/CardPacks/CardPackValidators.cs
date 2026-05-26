using FluentValidation;
using Bunker.ContentService.Api.CardPacks.Endpoints.Requests;

namespace Bunker.ContentService.Validation.Requests.CardPacks;

public class CreateCardPackValidator : AbstractValidator<CardPackRequest.Post.Create>
{
    public CreateCardPackValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10);
        RuleFor(x => x.CardIds).NotNull();
    }
}

public class UpdateCardPackValidator : AbstractValidator<CardPackRequest.Put.Update>
{
    public UpdateCardPackValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10);
        RuleFor(x => x.CardIds).NotNull();
    }
}
