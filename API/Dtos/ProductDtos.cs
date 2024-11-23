using FluentValidation;

namespace API.Dtos;

public record ProductGetDto(
  int Id,
  string Name,
  string SecondaryName,
  string Description,
  decimal SalePrice,
  decimal Rating,
  int StockQuantity,
  List<string> ImageUrls,
  int CategoryId
  );


public record ProductPostDto(
   string Name,
    string SecondaryName,
    string Description,
    decimal CostPrice,
    decimal SalePrice,
    decimal Rating,
    int StockQuantity,
    List<string> ImageUrls,
    int CategoryId
    );


public class ProductPostDtoValid : AbstractValidator<ProductPostDto> {
  public ProductPostDtoValid() {
    RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
    RuleFor(x => x.SecondaryName).NotEmpty().MaximumLength(50);
    RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
    RuleFor(x => x.CostPrice).GreaterThan(0);
    RuleFor(x => x.SalePrice).GreaterThan(0);
    RuleFor(x => x.Rating).GreaterThan(0);
    RuleFor(x => x.StockQuantity).GreaterThan(0);
    RuleFor(x => x.ImageUrls).NotEmpty();
    RuleFor(x => x.CategoryId).GreaterThan(0);
  }
}