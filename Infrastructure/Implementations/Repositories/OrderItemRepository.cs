using Core.Interfaces.Repositories;
using Core.Entities;

namespace Infrastructure.Implementations.Repositories;
public class OrderItemRepository(AppDbContext context) : Repository<OrderItem>(context), IOrderItemRepository {
}
