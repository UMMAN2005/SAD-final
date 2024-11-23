using FluentValidation;

namespace API.Dtos;

public record CardGetDto(
   int Id,
   string Holder,
   string Number,
   string AppUserId
   );

public record CardPostDto(
  string Holder,
  string Number,
  string Expiry,
  string CVV
  );


public class CardPostDtoValid : AbstractValidator<CardPostDto> {
  public CardPostDtoValid()
  {
    RuleFor(x => x.Holder).NotEmpty().MaximumLength(100);
    RuleFor(x => x.Number).NotEmpty().MaximumLength(16);
    RuleFor(x => x.Expiry).NotEmpty().MaximumLength(5);
    RuleFor(x => x.CVV).NotEmpty().MaximumLength(3);
  }
}
