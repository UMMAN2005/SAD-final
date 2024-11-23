using Core.Entities;
using Core.Interfaces.Repositories;

namespace Infrastructure.Implementations.Repositories;
public class ProductRepository(AppDbContext context) : Repository<Product>(context), IProductRepository {
}
