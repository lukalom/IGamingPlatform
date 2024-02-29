using IGamingPlatform.Domain;
using IGamingPlatform.Infrastructure.Repositories.Abstractions;
using IGamingPlatform.Infrastructure.UnitOfWork.Abstractions;
using IGamingPlatform.Shared;
using IGamingPlatform.Shared.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace IGamingPlatform.Application.Users.Commands.RegisterCommand;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<User> _userRepository;
    private readonly string _jwtSecretKey;

    public RegisterUserCommandHandler(IUnitOfWork unitOfWork, IRepository<User> userRepository, IOptions<JwtSettings> jwtSettings)
    {
        _jwtSecretKey = jwtSettings.Value.Key;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var (hashedPassword, salt) = PasswordHelper.HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = hashedPassword,
            Salt = salt
        };

        await _userRepository.StoreAsync(user);
        await _unitOfWork.SaveAsync();

        var token = JwtService.GenerateToken(user, _jwtSecretKey);

        return new RegisterUserResponse("User registered successfully", token);
    }
}
