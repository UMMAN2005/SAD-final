namespace API.Dtos;

public record OrderItemGetDto(
  int Id,
  int Quantity,
  int ProductId,
  decimal TotalPrice
  );
