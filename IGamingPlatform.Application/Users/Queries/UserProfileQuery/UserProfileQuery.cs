using IGamingPlatform.Application.Bets.Commands;
using MediatR;

namespace IGamingPlatform.Application.Users.Queries.UserProfileQuery;

public class UserProfileDto
{
    public string Username { get; set; }

    public string Email { get; set; }

    public decimal Balance { get; set; }

    public List<BetDto>? BetDtos { get; set; }

}

public record UserProfileQuery(int Id) : IRequest<UserProfileDto>;
