using IGamingPlatform.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IGamingPlatform.Infrastructure.Persistence.Configuration;

public class BetConfiguration : IEntityTypeConfiguration<Bet>
{
    public void Configure(EntityTypeBuilder<Bet> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Amount).IsRequired();
        builder.Property(b => b.Details).IsRequired();
        builder.HasOne(b => b.User)
            .WithMany(u => u.Bets) 
            .HasForeignKey(b => b.UserId);
    }
}