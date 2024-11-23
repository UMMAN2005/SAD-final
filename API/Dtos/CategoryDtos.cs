using FluentValidation;

namespace API.Dtos;

public record CategoryGetDto(
  int Id,
  string Name
);

public record CategoryPostDto(
  string Name
);

public class CategoryPostDtoValid : AbstractValidator<CategoryPostDto> {
  public CategoryPostDtoValid() {
    RuleFor(p => p.Name).NotEmpty().MaximumLength(50);
  }
}