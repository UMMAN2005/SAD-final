using System.Reflection;
using System.Text.Json;
using Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options) {
  public DbSet<Product> Products { get; set; }
  public DbSet<Category> Categories { get; set; }
  public DbSet<Review> Reviews { get; set; }
  public DbSet<OrderItem> OrderItems { get; set; }
  public DbSet<Order> Orders { get; set; }
  public DbSet<Card> Cards { get; set; }
  public DbSet<Payment> Payments { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    base.OnModelCreating(modelBuilder);

    var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
      v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    var addressConverter = new ValueConverter<Address, string>(
      v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
      v => JsonSerializer.Deserialize<Address>(v, (JsonSerializerOptions)null!)!);

    modelBuilder.Entity<Order>(entity => {
      entity.Property(o => o.ShippingAddress)
        .HasConversion(addressConverter);
    });

    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}