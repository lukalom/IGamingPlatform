using System.ComponentModel.DataAnnotations;
using IGamingPlatform.Domain.Enums;

namespace IGamingPlatform.Domain.Abstractions;

public abstract class Entity
{
    [Key]
    //public int Id { get; protected set; }
    public int Id { get;  set; }


    public EntityStatus EntityStatus { get; private set; } = EntityStatus.Active;
    public DateTimeOffset CreateDate { get; init; } = DateTimeOffset.Now.ToUniversalTime();

    [ConcurrencyCheck]
    public DateTimeOffset? LastChangeDate { get; protected set; }

    protected Entity()
    {
    }
}