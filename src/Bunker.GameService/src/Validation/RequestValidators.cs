using Bunker.GameService.Api.Endpoints.Request;
using FluentValidation;

namespace Bunker.GameService.Validation;

internal sealed class RevealAttributeRequestValidator : AbstractValidator<RevealAttribute>
{
    private static readonly HashSet<string> ValidKinds =
        ["Profession", "Hobbies", "Age", "Sex", "Fact", "Health", "Luggage"];

    public RevealAttributeRequestValidator()
    {
        RuleFor(x => x.AttributeKind)
            .NotEmpty()
            .Must(kind => ValidKinds.Contains(kind))
            .WithMessage("AttributeKind must be one of: Profession, Hobbies, Age, Sex, Fact, Health, Luggage.");
    }
}

internal sealed class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(500);
    }
}

internal sealed class VoteRequestValidator : AbstractValidator<VoteRequest>
{
    public VoteRequestValidator()
    {
        RuleFor(x => x.TargetParticipantId)
            .NotEmpty();
    }
}