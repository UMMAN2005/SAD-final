using FluentValidation;

namespace API.Dtos;

public class ReviewGetDto {
  public int Id { get; set; }
  public string AppUserId { get; set; }
  public string AppUserName { get; set; }
  public int ProductId { get; set; }
  public string Text { get; set; }
  public decimal Rating { get; set; }
  public DateTime CreatedAt { get; set; }

  public ReviewGetDto(int id, string appUserId, string appUserName, int productId, string text, decimal rating, DateTime createdAt) {
    Id = id;
    AppUserId = appUserId;
    AppUserName = appUserName;
    ProductId = productId;
    Text = text;
    Rating = rating;
    CreatedAt = createdAt;
  }

  public ReviewGetDto() {
  }
}

public class ReviewPostDto {
  public int ProductId { get; set; }
  public string Text { get; set; }
  public decimal Rating { get; set; }

  public ReviewPostDto(int productId, string text, decimal rating) {
    ProductId = productId;
    Text = text;
    Rating = rating;
  }

  public ReviewPostDto() {
  }
}


public class ReviewPostDtoValid : AbstractValidator<ReviewPostDto> {
  public ReviewPostDtoValid() {
    RuleFor(x => x.ProductId).GreaterThan(0);
    RuleFor(x => x.Text).NotEmpty().MaximumLength(500);
    RuleFor(x => x.Rating).GreaterThan(0);
  }
}