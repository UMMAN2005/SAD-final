using Core.Entities;
using FluentValidation;

namespace API.Dtos;

public record PaymentGetDto(
  int Id,
  decimal Amount,
  PaymentMethod Method,
  PaymentStatus Status,
  int OrderId
  );


public record PaymentPostDto(
    decimal Amount,
  PaymentMethod Method,
  PaymentStatus Status,
  int OrderId
  );

public record PaymentIntentDto(
  long Amount,
  string Currency
  );

public class PaymentPostDtoValid : AbstractValidator<PaymentPostDto> {
  public PaymentPostDtoValid() {
    RuleFor(x => x.Amount).GreaterThan(0);
    RuleFor(x => x.Method).IsInEnum();
    RuleFor(x => x.Status).IsInEnum();
    RuleFor(x => x.OrderId).GreaterThan(0);
  }
}

public class PaymentIntentDtoValid : AbstractValidator<PaymentIntentDto> {
  public PaymentIntentDtoValid() {
    RuleFor(x => x.Amount).GreaterThan(0);
    RuleFor(x => x.Currency).NotEmpty();
  }
}
