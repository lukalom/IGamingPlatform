using FluentValidation;
using IGamingPlatform.Domain;
using IGamingPlatform.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace IGamingPlatform.Application.Users.Commands.RegisterCommand;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly IRepository<User> _userRepository;
    public RegisterUserCommandValidator(IRepository<User> userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Passwords must match");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(BeUniqueEmailAsync)
            .WithMessage("Email already exists");

        RuleFor(x => x.Username)
            .NotEmpty()
            .NotNull()
            .MustAsync(BeUniqueUsernameAsync)
            .WithMessage("Username already exists");

    }

    private async Task<bool> BeUniqueEmailAsync(string email, CancellationToken cancellationToken)
        => !await _userRepository.Query(u => u.Email == email).AnyAsync(cancellationToken);

    private async Task<bool> BeUniqueUsernameAsync(string username, CancellationToken cancellationToken)
        => !await _userRepository.Query(u => u.Username == username).AnyAsync(cancellationToken);
}

