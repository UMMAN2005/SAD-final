using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;
public class PaymentConfig : IEntityTypeConfiguration<Payment> {
  public void Configure(EntityTypeBuilder<Payment> builder)
  {
    builder.Property(x => x.Method).IsRequired();
    builder.Property(x => x.Status).IsRequired();
    builder.Property(x => x.Amount).HasColumnType("decimal(18,2)").IsRequired();
    builder.Property(x => x.OrderId).IsRequired();

    builder.HasOne(x => x.Order).WithMany(x => x.Payments).HasForeignKey(x => x.OrderId);
  }
}
