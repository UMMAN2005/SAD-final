using System.Linq.Expressions;
using Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations.Repositories;

public class Repository<T>(AppDbContext context) : IRepository<T>
  where T : class, new() {
  public async Task AddAsync(T entity) {
    await context.Set<T>().AddAsync(entity);
  }

  public async Task DeleteAsync(T entity) {
    await Task.Run(() => context.Set<T>().Remove(entity));
  }

  public async Task<IQueryable<T>> GetPaginatedAsync(Expression<Func<T, bool>> predicate, int pageNumber = 1, int pageSize = 1, params string[] includes) {
    var query = context.Set<T>().AsQueryable();

    query = includes.Aggregate(query, (current, include) => current.Include(include));

    query = query.Where(predicate);

    return await Task.FromResult(query);
  }

  public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, params string[] includes) {
    var query = context.Set<T>().AsQueryable();

    query = includes.Aggregate(query, (current, include) => current.Include(include));

    return await query.FirstOrDefaultAsync(predicate);
  }

  public async Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, params string[] includes) {
    var query = context.Set<T>().AsQueryable();

    query = includes.Aggregate(query, (current, include) => current.Include(include));

    query = query.Where(predicate);

    return await Task.FromResult(query);
  }

  public async Task SaveAsync() {
    await context.SaveChangesAsync();
  }
}