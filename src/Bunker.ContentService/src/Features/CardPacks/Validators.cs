using FluentValidation;

namespace Bunker.ContentService.Features.CardPacks;

public class CreateCardPackValidator : AbstractValidator<CreateCardPack>
{
    public CreateCardPackValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10);
        RuleFor(x => x.CardIds).NotNull();
    }
}

public class UpdateCardPackValidator : AbstractValidator<UpdateCardPack>
{
    public UpdateCardPackValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10);
        RuleFor(x => x.CardIds).NotNull();
    }
}

public class AddCardToPackValidator : AbstractValidator<AddCardToPack>
{
    public AddCardToPackValidator()
    {
        RuleFor(x => x.CardPackId).NotEmpty();
        RuleFor(x => x.CardId).NotEmpty();
    }
}

public class RemoveCardFromPackValidator : AbstractValidator<RemoveCardFromPack>
{
    public RemoveCardFromPackValidator()
    {
        RuleFor(x => x.CardPackId).NotEmpty();
        RuleFor(x => x.CardId).NotEmpty();
    }
}
