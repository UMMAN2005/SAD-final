using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;
public class OrderItemConfig : IEntityTypeConfiguration<OrderItem> {
  public void Configure(EntityTypeBuilder<OrderItem> builder) {
    builder.Property(x => x.Quantity).IsRequired();
    builder.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();

    builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
    builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId);
  }
}
