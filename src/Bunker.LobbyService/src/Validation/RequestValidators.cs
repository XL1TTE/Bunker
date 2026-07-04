using Bunker.LobbyService.Api.Endpoints.Request;
using FluentValidation;

namespace Bunker.LobbyService.Validation;


internal sealed class CreateLobbyRequestValidator : AbstractValidator<CreateLobby>
{
    public CreateLobbyRequestValidator()
    {
        RuleFor(x => x.Capacity)
            .GreaterThan(4);

        RuleFor(x => x.CardPackIds)
            .Must(ids => ids.All(id => Guid.TryParse(id, out _)))
            .WithMessage("CardPackIds must contain only guid strings.");
    }
}
