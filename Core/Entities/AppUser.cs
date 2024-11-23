using Microsoft.AspNetCore.Identity;
namespace Core.Entities;

public enum Gender {
  Male,
  NotMale,
  OtherThanMale,
  PreferNotToSay
}


public class AppUser : IdentityUser {
  public Gender Gender { get; set; }
  public DateTime Birthday { get; set; }
  public string AvatarUrl { get; set; } = default!;
  public ICollection<Order> Orders { get; set; } = [];
  public ICollection<Review> Reviews { get; set; } = [];
  public ICollection<Card> Cards { get; set; } = [];
}