using FluentValidation;

namespace Bunker.AccountService.Features.UpdateProfile;

internal class UpdateProfileValidator : AbstractValidator<UpdateProfile>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .Matches(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")
            .WithMessage("Invalid Player ID format. Must be a valid GUID.");

        RuleFor(x => x.Nickname)
            .NotEmpty()
            .Length(3, 32)
            .WithMessage("Nickname must be between 3 and 32 characters.");
    }
}