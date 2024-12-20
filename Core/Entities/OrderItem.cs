namespace Core.Entities;

public class OrderItem : BaseEntity {
  public int Quantity { get; set; } = default!;
  public Product Product { get; set; } = default!;
  public int ProductId { get; set; } 
  public Order Order { get; set; } = default!;
  public int OrderId {get; set; } = default!;
  public decimal TotalPrice { get; set; } = default!;
}