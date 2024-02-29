namespace IGamingPlatform.UnitTests;

using IGamingPlatform.Application.Bets.Commands;
using IGamingPlatform.Domain;
using IGamingPlatform.Infrastructure.Repositories.Abstractions;
using IGamingPlatform.Infrastructure.UnitOfWork.Abstractions;
using IGamingPlatform.Shared.Exceptions;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class PlaceBetCommandHandlerTests
{
    private readonly Mock<IRepository<Bet>> _mockBetRepository;
    private readonly Mock<IRepository<User>> _mockUserRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly PlaceBetCommandHandler _handler;

    public PlaceBetCommandHandlerTests()
    {
        _mockBetRepository = new Mock<IRepository<Bet>>();
        _mockUserRepository = new Mock<IRepository<User>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _handler = new PlaceBetCommandHandler(_mockBetRepository.Object, _mockUserRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidBet_PlacesBetAndUpdatesBalance()
    {
        // Arrange
        var userId = 1;
        var amount = 100;
        var details = "Bet details";
        var user = new User { Id = userId, Balance = 200 };
        var command = new PlaceBetCommand(userId, new PlaceBetModel(amount, details));

        _mockUserRepository.Setup(repo => repo.FindAsync(userId, true))
            .ReturnsAsync(user);

        var bet = new Bet
        {
            Amount = amount,
            Details = details,
            User = user,
            PlacedAt = DateTime.UtcNow
        };

        var bets = new List<Bet> { bet };
        var betDtos = bets.Select(x => new BetDto(x.Id, x.Amount, x.Details, x.PlacedAt)).ToList();

        var mock = bets.AsQueryable().BuildMock();
        _mockBetRepository.Setup(repo => repo.Query(
                It.IsAny<Expression<Func<Bet, bool>>>(),
                It.IsAny<bool>()
                ))
            .Returns(mock);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Balance, result.NewBalance);
        Assert.Equal(betDtos.Count, result.Bets.Count);
        _mockUserRepository.Verify(repo => repo.FindAsync(userId, true), Times.Once);
        _mockBetRepository.Verify(repo => repo.StoreAsync(It.IsAny<Bet>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var userId = 1;
        var command = new PlaceBetCommand(userId, new PlaceBetModel(100, "Bet details"));

        _mockUserRepository.Setup(repo => repo.FindAsync(userId, true))
            .ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InsufficientBalance_ThrowsInsufficientBalanceException()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, Balance = 50 };
        var command = new PlaceBetCommand(userId, new PlaceBetModel(100, "Bet details"));

        _mockUserRepository.Setup(repo => repo.FindAsync(userId, true))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<InsufficientBalanceException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
