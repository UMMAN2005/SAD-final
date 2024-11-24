namespace API.Dtos;

public record OrderItemGetDto(
  int Id,
  int Quantity,
  int ProductId,
  decimal TotalPrice
);

public record OrderItemPostDto(
   int Quantity,
    int ProductId,
    decimal TotalPrice
   );