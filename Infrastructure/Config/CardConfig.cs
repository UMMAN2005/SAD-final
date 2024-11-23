using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;
public class CardConfig : IEntityTypeConfiguration<Card> {
  public void Configure(EntityTypeBuilder<Card> builder)
  {
    builder.Property(x => x.Holder).IsRequired().HasMaxLength(100);
    builder.Property(x => x.Number).IsRequired().HasMaxLength(16);
    builder.Property(x => x.Expiry).IsRequired().HasMaxLength(5);
    builder.Property(x => x.CVV).IsRequired().HasMaxLength(3);

    builder.HasOne(x => x.AppUser).WithMany(x => x.Cards).HasForeignKey(x => x.AppUserId);
  }
}
