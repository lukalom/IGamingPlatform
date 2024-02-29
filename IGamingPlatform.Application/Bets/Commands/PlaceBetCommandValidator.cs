using FluentValidation;

namespace IGamingPlatform.Application.Bets.Commands;

public class PlaceBetCommandValidator : AbstractValidator<PlaceBetCommand>
{
    public PlaceBetCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Model.Amount).GreaterThan(0);
        RuleFor(x => x.Model.Details).NotEmpty();
    }
}