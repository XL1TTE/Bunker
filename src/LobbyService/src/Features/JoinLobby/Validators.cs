using FluentValidation;

namespace LobbyService.Features.JoinLobby;

internal class JoinLobbyRequestValidator : AbstractValidator<JoinLobbyRequest>
{
    public JoinLobbyRequestValidator()
    {
        RuleFor(x => x.InviteCode)
            .NotEmpty()
            .Length(6)
            .Matches("^[A-Z0-9]*$")
            .WithMessage("Invite code must be 6 alphanumeric characters.");
    }
}
