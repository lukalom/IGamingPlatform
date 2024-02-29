using IGamingPlatform.Domain;
using IGamingPlatform.Infrastructure.Repositories.Abstractions;
using IGamingPlatform.Shared;
using IGamingPlatform.Shared.Exceptions;
using IGamingPlatform.Shared.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IGamingPlatform.Application.Users.Commands.LoginCommand;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
{
    private readonly IRepository<User> _userRepository;
    private readonly string _jwtSecretKey;

    public LoginUserCommandHandler(IRepository<User> userRepository, IOptions<JwtSettings> jwtSettings)
    {
        _jwtSecretKey = jwtSettings.Value.Key;
        _userRepository = userRepository;
    }

    public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.Query(x => x.Username == request.Username).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (user == null)
        {
            throw new NotFoundException($"User with this username '{request.Username}' doesn't exist");
        }

        if (!PasswordHelper.VerifyPassword(request.Password, user.PasswordHash, user.Salt))
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        var token = JwtService.GenerateToken(user, _jwtSecretKey);

        return new LoginUserResponse(user.Id, user.Username, token);
    }
}
