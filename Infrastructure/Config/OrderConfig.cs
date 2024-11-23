using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;
public class OrderConfig : IEntityTypeConfiguration<Order> {
  public void Configure(EntityTypeBuilder<Order> builder)
  {
    builder.Property(x => x.ShippingAddress).IsRequired();
    builder.Property(x => x.ShippingPrice).HasColumnType("decimal(18,2)");
    builder.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)");
    builder.Property(x => x.SubtotalPrice).HasColumnType("decimal(18,2)");

    builder.HasMany(x => x.Items).WithOne().OnDelete(DeleteBehavior.Cascade);
    builder.HasMany(x => x.Payments).WithOne().OnDelete(DeleteBehavior.Cascade);
  }
}
