
using Bunker.AccountService.Features.CreateProfile;
using FluentValidation;

internal sealed class CreateProfileValidator : AbstractValidator<CreateProfile>
{
    public CreateProfileValidator()
    {
        RuleFor(x => x.Nickname)
            .MaximumLength(32)
            .MinimumLength(4);
    }
}
