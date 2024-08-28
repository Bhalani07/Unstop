using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UnstopAPI.Data;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }


        #region Read

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, string includeProperties = null, int pageSize = 5, int pageNumber = 1, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if(orderBy != null)
            {
                query = orderBy(query);
            }

            if (pageNumber > 0)
            {
                if (pageSize > 20)
                {
                    pageSize = 20;
                }

                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }

            if (includeProperties != null)
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<int> GetTotalRecordsAsync(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.CountAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        #endregion


        #region Create 

        public async Task CreateAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await SaveAsync();
        }

        #endregion


        #region Delete

        public async Task DeleteAsync(T entity)
        {
            dbSet.Remove(entity);
            await SaveAsync();
        }

        #endregion


        #region Update

        public async Task UpdateAsync(T entity)
        {
            dbSet.Update(entity);
            await SaveAsync();
        }

        public async Task UpdatePartialAsync(T entity, params Expression<Func<T, object>>[] updatedProperties)
        {
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }

            foreach (var property in updatedProperties)
            {
                entry.Property(property).IsModified = true;
            }

            await SaveAsync();
        }

        #endregion


        #region Save CRUD

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
