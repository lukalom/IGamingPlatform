using IGamingPlatform.Domain;
using IGamingPlatform.Infrastructure.Repositories.Abstractions;
using IGamingPlatform.Infrastructure.UnitOfWork.Abstractions;
using IGamingPlatform.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IGamingPlatform.Application.Bets.Commands;

public class PlaceBetCommandHandler : IRequestHandler<PlaceBetCommand, PlaceBetResponse>
{
    private readonly IRepository<Bet> _betRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PlaceBetCommandHandler(IRepository<Bet> betRepository, IRepository<User> userRepository, IUnitOfWork unitOfWork)
    {
        _betRepository = betRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PlaceBetResponse> Handle(PlaceBetCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindAsync(request.UserId);

        if (user == null)
        {
            throw new NotFoundException();
        }

        if (user.Balance < request.Model.Amount)
        {
            throw new InsufficientBalanceException("Insufficient balance");
        }
        
        var bet = new Bet
        {
            Amount = request.Model.Amount,
            Details = request.Model.Details,
            User = user,
            PlacedAt = DateTime.UtcNow
        };

        user.Balance -= request.Model.Amount;

        user.Bets?.Add(bet);

        await _betRepository.StoreAsync(bet);
        await _unitOfWork.SaveAsync();

        var bets = await _betRepository.Query(x => x.UserId == user.Id)
           .Select(x => new BetDto(x.Id, x.Amount, x.Details, x.PlacedAt))
           .AsNoTracking()
           .ToListAsync(cancellationToken: cancellationToken);

       return new PlaceBetResponse(user.Balance, bets);
    }
}