using IGamingPlatform.Application.Bets.Commands;
using IGamingPlatform.Domain;
using IGamingPlatform.Infrastructure.Repositories.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IGamingPlatform.Application.Users.Queries.UserProfileQuery;

public class UserQueryHandler : IRequestHandler<UserProfileQuery, UserProfileDto>
{
    private readonly IRepository<User> _userRepository;

    public UserQueryHandler(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileDto> Handle(UserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.Query(x => x.Id == request.Id)
            .Include(x=>x.Bets)
            .FirstOrDefaultAsync(cancellationToken);

        return new UserProfileDto
        {
            Email = user.Email,
            Username = user.Username,
            Balance = user.Balance,
            BetDtos = user.Bets?.Select(x=> new BetDto(x.Id, x.Amount, x.Details, x.PlacedAt)).ToList()
        };
    }
}