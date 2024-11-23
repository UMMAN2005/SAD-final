using Core.Entities;
using FluentValidation;

namespace API.Dtos;

public record OrderGetDto(
  int Id,
  decimal ShippingPrice,
  decimal TotalPrice,
  decimal SubtotalPrice,
  string AppUserId,
  Address ShippingAddress,
  IReadOnlyList<OrderItemGetDto> Items,
  IReadOnlyList<PaymentGetDto> Payments
);


public record OrderPostDto(
  decimal ShippingPrice,
  decimal TotalPrice,
  decimal SubtotalPrice,
  Address ShippingAddress,
  IReadOnlyList<OrderItemGetDto> Items,
  IReadOnlyList<PaymentGetDto> Payments
);

public class OrderPostDtoValid : AbstractValidator<OrderPostDto> {
  public OrderPostDtoValid() {
    RuleFor(x => x.ShippingPrice).GreaterThan(0);
    RuleFor(x => x.TotalPrice).GreaterThan(0);
    RuleFor(x => x.SubtotalPrice).GreaterThan(0);
    RuleFor(x => x.ShippingAddress).NotEmpty();
    RuleFor(x => x.Items).NotNull();
    RuleFor(x => x.Payments).NotNull();
  }
}