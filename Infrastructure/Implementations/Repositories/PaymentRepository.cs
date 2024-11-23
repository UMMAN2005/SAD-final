using Core.Entities;
using Core.Interfaces.Repositories;

namespace Infrastructure.Implementations.Repositories;
public class PaymentRepository(AppDbContext context) : Repository<Payment>(context), IPaymentRepository {
}
