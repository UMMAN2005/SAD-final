namespace Core.Entities;

public enum PaymentMethod {
  Card,
  Cash
}

public enum PaymentStatus {
  Pending,
  Approved,
  Declined
}

public class Payment : BaseEntity {
  public decimal Amount { get; set; }
  public PaymentMethod Method { get; set; } = default!;
  public PaymentStatus Status { get; set; } = default!;
  public int OrderId { get; set; }
  public Order Order { get; set; } = default!;
}
