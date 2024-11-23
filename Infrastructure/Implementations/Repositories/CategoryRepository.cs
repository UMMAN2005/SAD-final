using Core.Entities;
using Core.Interfaces.Repositories;

namespace Infrastructure.Implementations.Repositories;
public class CategoryRepository(AppDbContext context) : Repository<Category>(context), ICategoryRepository {
}
