using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Config;

public class ReviewConfig : IEntityTypeConfiguration<Review> {
  public void Configure(EntityTypeBuilder<Review> builder) {
    builder.Property(p => p.AppUserId).IsRequired();
    builder.Property(p => p.ProductId).IsRequired();
    builder.Property(p => p.Text).IsRequired().HasMaxLength(500);
    builder.Property(p => p.Rating).HasColumnType("decimal(18,2)").IsRequired();

    builder.HasOne(u => u.Product)
        .WithMany(u => u.Reviews)
        .HasForeignKey(u => u.ProductId);

    builder.HasOne(u => u.AppUser)
        .WithMany(u => u.Reviews)
        .HasForeignKey(u => u.AppUserId);
  }
}