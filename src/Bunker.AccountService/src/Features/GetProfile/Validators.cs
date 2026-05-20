using FluentValidation;

namespace Bunker.AccountService.Features.GetProfile;

internal class GetProfileValidator : AbstractValidator<GetProfile>
{
    public GetProfileValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .Matches(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")
            .WithMessage("Invalid Player ID format. Must be a valid GUID.");
    }
}
