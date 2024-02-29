using IGamingPlatform.Domain.Abstractions;

namespace IGamingPlatform.Domain;

public class User : Entity
{
    public string Username { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string PasswordHash { get; set; } = default!;

    public string Salt { get; set; } = default!;

    public decimal Balance { get; set; }  

    public List<Bet>? Bets { get; set; } = new();
}