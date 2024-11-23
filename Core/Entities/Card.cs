namespace Core.Entities;
public class Card : BaseEntity {
  public string Holder { get; set; } = default!;
  public string Number { get; set; } = default!;
  public string Expiry { get; set; } = default!;
  public string CVV { get; set; } = default!;
  public string AppUserId { get; set; } = default!;
  public AppUser AppUser { get; set; } = default!;
}
