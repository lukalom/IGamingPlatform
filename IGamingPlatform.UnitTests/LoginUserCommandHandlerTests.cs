using IGamingPlatform.Application.Users.Commands.LoginCommand;
using IGamingPlatform.Domain;
using IGamingPlatform.Infrastructure.Repositories.Abstractions;
using IGamingPlatform.Shared;
using IGamingPlatform.Shared.Exceptions;
using IGamingPlatform.Shared.Settings;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;

namespace IGamingPlatform.UnitTests;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IRepository<User>> _mockUserRepository;
    private readonly LoginUserCommandHandler _handler;
    private readonly string _jwtSecretKey = "kyA7f9i6CbCMf87dmmgNclQ7rNOXHrlABBBBBBBBB";

    public LoginUserCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IRepository<User>>();

        var jwtSettings = Options.Create(new JwtSettings { Key = _jwtSecretKey });
        _handler = new LoginUserCommandHandler(_mockUserRepository.Object, jwtSettings);
    }

    [Fact]
    public async Task Handle_ValidUser_ReturnsLoginUserResponse()
    {
        // Arrange
        var username = "testuser";
        var password = "password";
        var (hashedPassword, salt) = PasswordHelper.HashPassword(password);

        var command = new LoginUserCommand(username, password);
        var user = new User { Id = 1, Username = username, PasswordHash = hashedPassword, Salt = salt };

        var users = new List<User>
        {
            user
        };

        var mock = users.AsQueryable().BuildMock();
        _mockUserRepository.Setup(repo => repo.Query(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<bool>()))
            .Returns(mock);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.Username, result.Username);
        Assert.NotNull(result.Token);
    }

    [Fact]
    public async Task Handle_InvalidUsername_ThrowsNotFoundException()
    {
        // Arrange
        var command = new LoginUserCommand("invaliduser", "password");

        var mock = Enumerable.Empty<User>().AsQueryable().BuildMock();
        _mockUserRepository.Setup(repo => repo.Query(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<bool>()))
            .Returns(mock);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }


    [Fact]
    public async Task Handle_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var username = "testuser";
        var user = new User { Id = 1, Username = username, PasswordHash = "hashedpassword", Salt = "salt" };
        var command = new LoginUserCommand(username, "wrongpassword");

        _mockUserRepository.Setup(repo => repo.Query(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<bool>()))
            .Returns(new[] { user }.AsQueryable().BuildMock);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
