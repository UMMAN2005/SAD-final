using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface IOrderRepository : IRepository<Order> {
  Task AddOrderItemAsync(OrderItem orderItem);
  Task DeleteOrderItemAsync(OrderItem orderItem);
}
