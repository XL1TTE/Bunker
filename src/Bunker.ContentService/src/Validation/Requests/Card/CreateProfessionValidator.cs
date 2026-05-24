
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using FluentValidation;

namespace Bunker.ContentService.Api.Validation;

internal sealed class CreateProfessionCardValidator : AbstractValidator<CardRequest.Post.ProfessionCard>
{
    public CreateProfessionCardValidator()
    {
        
    }
}
