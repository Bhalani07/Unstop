using System.Linq.Expressions;

namespace UnstopAPI.Repository.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, string includeProperties = null, int pageSize = 5, int pageNumber = 1, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);

        Task<int> GetTotalRecordsAsync(Expression<Func<T, bool>> filter = null);

        Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, string includeProperties = null);

        Task CreateAsync(T entity);

        Task UpdateAsync(T entity);

        Task UpdatePartialAsync(T entity, params Expression<Func<T, object>>[] updatedProperties);

        Task DeleteAsync(T entity);

        Task SaveAsync();
    }
}
