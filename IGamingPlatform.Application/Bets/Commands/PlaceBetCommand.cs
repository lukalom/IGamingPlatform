using MediatR;

namespace IGamingPlatform.Application.Bets.Commands;

public record PlaceBetCommand(int UserId, PlaceBetModel Model) : IRequest<PlaceBetResponse>;

public record PlaceBetModel(decimal Amount, string Details);

public record BetDto(int Id, decimal Amount, string Details, DateTimeOffset PlacedAt);

public record PlaceBetResponse(decimal NewBalance, List<BetDto> Bets);