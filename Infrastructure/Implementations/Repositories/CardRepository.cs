using Core.Entities;
using Core.Interfaces.Repositories;

namespace Infrastructure.Implementations.Repositories;
public class CardRepository(AppDbContext context) : Repository<Card>(context), ICardRepository {
}
