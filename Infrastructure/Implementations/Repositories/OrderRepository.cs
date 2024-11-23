using Core.Entities;
using Core.Interfaces.Repositories;

namespace Infrastructure.Implementations.Repositories;
public class OrderRepository(AppDbContext context) : Repository<Order>(context), IOrderRepository {
}
