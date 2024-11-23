namespace Core.Entities;

public class Product : BaseEntity {
  public string Name { get; set; } = default!;
  public string SecondaryName { get; set; } = default!;
  public string Description { get; set; } = default!;
  public decimal CostPrice { get; set; }
  public decimal SalePrice { get; set; }
  public decimal Rating { get; set; }
  public int StockQuantity { get; set; }
  public ICollection<Review> Reviews { get; set; } = [];
  public ICollection<OrderItem> OrderItems { get; set; } = [];
  public List<string> ImageUrls { get; set; } = [];
  public int CategoryId { get; set; }
  public Category Category { get; set; } = default!;
}