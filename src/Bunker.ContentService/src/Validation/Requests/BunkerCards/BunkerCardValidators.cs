using FluentValidation;
using Bunker.ContentService.Api.BunkerCards.Endpoints.Requests;

namespace Bunker.ContentService.Validation.Requests.BunkerCards;

public class CreateBunkerCardValidator : AbstractValidator<BunkerCardRequest.Post.Create>
{
    public CreateBunkerCardValidator()
    {
        RuleFor(x => x.Catastrophe).NotEmpty().MinimumLength(8).MaximumLength(255);
        RuleFor(x => x.SurvivalDuration).NotEmpty().MinimumLength(3).MaximumLength(64);
        RuleFor(x => x.BunkerEnvironment).NotEmpty().MinimumLength(10).MaximumLength(2000);
    }
}

public class UpdateBunkerCardValidator : AbstractValidator<BunkerCardRequest.Put.Update>
{
    public UpdateBunkerCardValidator()
    {
        RuleFor(x => x.Catastrophe).NotEmpty().MinimumLength(8).MaximumLength(255);
        RuleFor(x => x.SurvivalDuration).NotEmpty().MinimumLength(3).MaximumLength(64);
        RuleFor(x => x.BunkerEnvironment).NotEmpty().MinimumLength(10).MaximumLength(2000);
    }
}