namespace Core.Entities;

public record Address(
  string FirstName, string LastName, string Country, string Street, string City, string State, string Zipcode
);

public class Order : BaseEntity {
  public ICollection<OrderItem> Items { get; set; } = [];
  public decimal ShippingPrice { get; set; }
  public decimal TotalPrice { get; set; }
  public decimal SubtotalPrice { get; set; }
  public Address ShippingAddress { get; set; } = default!;
  public ICollection<Payment> Payments { get; set; } = [];
}