using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;
public class AppUserConfig : IEntityTypeConfiguration<AppUser> {
  public void Configure(EntityTypeBuilder<AppUser> builder) {
    builder.Property(x => x.Email).IsRequired().HasMaxLength(50);
    builder.Property(x => x.UserName).IsRequired().HasMaxLength(50);

    builder.HasIndex(x => x.Email).IsUnique();
    builder.HasIndex(x => x.UserName).IsUnique();

    builder.HasMany(x => x.Orders)
      .WithOne(y => y.AppUser)
      .HasForeignKey(y => y.AppUserId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(x => x.Cards)
      .WithOne(y => y.AppUser)
      .HasForeignKey(y => y.AppUserId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(x => x.Reviews)
      .WithOne(y => y.AppUser)
      .HasForeignKey(y => y.AppUserId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.Property(x => x.OtpCode)
      .HasMaxLength(6)
      .IsRequired(false);

    builder.Property(x => x.TotpSecret)
      .HasMaxLength(160)
      .IsRequired(false);

    builder.Property(x => x.OtpExpirationTime)
      .IsRequired(false);

    builder.Property(x => x.LastOtpRequestTime)
      .IsRequired(false);

    builder.Property(x => x.Provider)
      .HasConversion<string>()
      .IsRequired();

    builder.Property(x => x.Gender)
      .HasConversion<string>()
      .IsRequired();
  }
}
