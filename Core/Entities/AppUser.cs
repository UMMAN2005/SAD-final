using Microsoft.AspNetCore.Identity;
namespace Core.Entities;

public enum Gender {
  Male,
  NotMale,
  OtherThanMale,
  PreferNotToSay,
  NotSpecified
}

public enum AuthProvider {
  Local,
  Google,
  Facebook,
  Twitter
}


public class AppUser : IdentityUser {
  public Gender Gender { get; set; } = Gender.NotSpecified;
  public DateTime Birthday { get; set; } = DateTime.UtcNow;
  public string? AvatarUrl { get; set; }
  public ICollection<Order> Orders { get; set; } = [];
  public ICollection<Review> Reviews { get; set; } = [];
  public ICollection<Card> Cards { get; set; } = [];
  public AuthProvider Provider { get; set; } = AuthProvider.Local;
  public string OtpCode { get; set; } = default!;
  public string TotpSecret { get; set; } = default!;
  public DateTime? OtpExpirationTime { get; set; }
  public DateTime? LastOtpRequestTime { get; set; }
}