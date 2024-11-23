using FluentValidation;

namespace API.Dtos;

public record ReviewGetDto(
  int Id,
  string AppUserId,
  string AppUserName,
  int ProductId,
  string Text,
  decimal Rating,
  DateTime CreatedAt
);

public record ReviewPostDto(
  int ProductId,
  string Text,
  decimal Rating
);

public class ReviewPostDtoValid : AbstractValidator<ReviewPostDto> {
  public ReviewPostDtoValid() {
    RuleFor(x => x.ProductId).GreaterThan(0);
    RuleFor(x => x.Text).NotEmpty().MaximumLength(500);
    RuleFor(x => x.Rating).GreaterThan(0);
  }
}