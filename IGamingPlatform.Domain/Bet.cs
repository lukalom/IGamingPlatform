using IGamingPlatform.Domain.Abstractions;

namespace IGamingPlatform.Domain;

public class Bet : Entity
{
    public decimal Amount { get; set; }

    public string Details { get; set; } = default!;

    public DateTimeOffset PlacedAt { get; set; } = DateTimeOffset.Now;

    public int UserId { get; set; }
    public User? User { get; set; } 
}