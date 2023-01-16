using Microsoft.EntityFrameworkCore;
using ProjectManager.DAL.Context;
using ProjectManager.Domain.Interfaces;
using System.Linq.Expressions;

namespace ProjectManager.DAL.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private ManagerDBContext db;
        private DbSet<TEntity> dbSet;
        public Repository(ManagerDBContext context)
        {
            db = context;
            dbSet = db.Set<TEntity>();
        }

        public void Add(TEntity item)
        {
            dbSet.Add(item);
        }

        public async Task<TEntity> AddAsync(TEntity item)
        {
            await dbSet.AddAsync(item);
            return item;
        }

        public void Delete(TEntity item)
        {
            dbSet.Remove(item);
        }

        public async Task DeleteAsync(TEntity item)
        {
            dbSet.Remove(item);
        }

        public IQueryable<TEntity> GetAll()
        {
            return dbSet.AsQueryable().AsNoTracking();
        }

        public TEntity Get(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, object>>[]? incl)
        {
            var data = dbSet.Where(predicate);
            if (incl != null)
            {
                data = incl.Aggregate(data, (current, inclusion) => current.Include(inclusion));
            }
            return data.FirstOrDefault();
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, object>>[]? incl)
        {
            var data = dbSet.Where(predicate);
            if (incl != null)
            {
                data = incl.Aggregate(data, (current, inclusion) => current.Include(inclusion));
            }
            return data.FirstOrDefault();
        }

        public async Task<TEntity> GetNoTrackingAsync(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, object>>[]? incl = null)
        {
            var data = dbSet.Where(predicate).AsNoTracking();
            if (incl != null)
            {
                data = incl.Aggregate(data, (current, inclusion) => current.Include(inclusion));
            }
            return data.FirstOrDefault();
        }

        public TEntity Update(TEntity item)
        {
            db.Update(item);
            return item;
        }

        public async Task<TEntity> UpdateAsync(TEntity item)
        {
            db.Update(item);
            return item;
        }
    }
}
