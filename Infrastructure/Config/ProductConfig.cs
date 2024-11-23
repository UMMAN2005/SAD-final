using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class ProductConfig : IEntityTypeConfiguration<Product> {
  public void Configure(EntityTypeBuilder<Product> builder) {

    builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
    builder.Property(p => p.SecondaryName).IsRequired().HasMaxLength(100);
    builder.Property(p => p.Description).IsRequired().HasMaxLength(500);
    builder.Property(p => p.CostPrice).HasColumnType("decimal(18,2)").IsRequired();
    builder.Property(p => p.SalePrice).HasColumnType("decimal(18,2)").IsRequired();
    builder.Property(p => p.StockQuantity).IsRequired();
    builder.Property(p => p.Rating).IsRequired();
    builder.Property(p => p.ImageUrls).IsRequired();
    builder.Property(p => p.CategoryId).IsRequired();

    builder.HasOne(b => b.Category).WithMany(p => p.Products).HasForeignKey(p => p.CategoryId);
    builder.HasMany(p => p.Reviews).WithOne().HasForeignKey(p => p.ProductId);
    builder.HasIndex(p => p.Name).IsUnique();
  }
}