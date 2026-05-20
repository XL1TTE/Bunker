using Bunker.AccountService.Features.GetProfile;
using FluentValidation;

internal sealed class GetProfileValidator : AbstractValidator<GetProfile>
{
    public GetProfileValidator()
    {
        // Valid uuid
        RuleFor(x => x.Id).Matches(@"^[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$");
    }
}
