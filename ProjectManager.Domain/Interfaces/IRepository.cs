using System.Linq.Expressions;

namespace ProjectManager.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        T Get(Expression<Func<T, bool>>? predicate, params Expression<Func<T, object>>[]? incl);
        Task<T> GetAsync(Expression<Func<T, bool>>? predicate, params Expression<Func<T, object>>[]? incl);
        Task<T> GetNoTrackingAsync(Expression<Func<T, bool>>? predicate, params Expression<Func<T, object>>[]? incl);
        void Add(T item);
        Task<T> AddAsync(T item);
        T Update(T item);
        Task<T> UpdateAsync(T item);
        void Delete(T item);
        Task DeleteAsync(T item);
    }
}
