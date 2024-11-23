using Core.Entities;
using Core.Interfaces.Repositories;

namespace Infrastructure.Implementations.Repositories;
public class ReviewRepository(AppDbContext context) : Repository<Review>(context), IReviewRepository {
}
