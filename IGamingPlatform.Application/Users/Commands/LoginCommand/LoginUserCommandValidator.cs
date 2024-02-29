using FluentValidation;
using IGamingPlatform.Application.Users.Commands.RegisterCommand;

namespace IGamingPlatform.Application.Users.Commands.LoginCommand;

public class LoginUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotNull()
            .NotEmpty();
        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty();

    }
}