using System.Linq.Expressions;

namespace Core.Interfaces.Repositories;

public interface IRepository<T> where T : class {
  Task<IQueryable<T>> GetPaginatedAsync(Expression<Func<T, bool>> predicate, int pageNumber = 1, int pageSize = 1, params string[] includes);
  Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, params string[] includes);
  Task<T?> GetAsync(Expression<Func<T, bool>> predicate, params string[] includes);
  Task<bool> ExistAsync(Expression<Func<T, bool>> predicate);
  Task AddAsync(T entity);
  Task SaveAsync();
  Task DeleteAsync(T entity);
}