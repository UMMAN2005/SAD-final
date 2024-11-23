using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class CategoryConfig : IEntityTypeConfiguration<Category> {
  public void Configure(EntityTypeBuilder<Category> builder) {
    builder.Property(p => p.Name).IsRequired().HasMaxLength(50);

    builder.HasMany(p => p.Products).WithOne(p => p.Category).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Cascade);
    builder.HasIndex(p => p.Name).IsUnique();
  }
}