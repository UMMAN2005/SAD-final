using Core.Entities;
using Core.Interfaces.Repositories;

namespace Infrastructure.Implementations.Repositories;
public class OrderRepository(AppDbContext context) : Repository<Order>(context), IOrderRepository {
  private readonly AppDbContext _context = context;

  public async Task AddOrderItemAsync(OrderItem orderItem) {
    await _context.OrderItems.AddAsync(orderItem);
    await _context.SaveChangesAsync();
  }

  public async Task DeleteOrderItemAsync(OrderItem orderItem) {
    _context.OrderItems.Remove(orderItem);
    await _context.SaveChangesAsync();
  }
}
