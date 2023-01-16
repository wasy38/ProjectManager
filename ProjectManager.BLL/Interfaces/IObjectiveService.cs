using ProjectManager.BLL.DTO;
using ProjectManager.Domain.Entities;
using System.Linq.Expressions;

namespace ProjectManager.BLL.Interfaces
{
    public interface IObjectiveService
    {
        void Add(ObjectiveDTO item);
        Task<ObjectiveDTO> AddAsync(ObjectiveDTO item);
        ObjectiveDTO Get(Expression<Func<Objective, bool>>? predicate);
        Task<ObjectiveDTO> GetAsync(Expression<Func<Objective, bool>>? predicate);
        ICollection<ObjectiveDTO> GetAll();
        void Delete(Guid id);
        Task DeleteAsync(Guid id);
        ObjectiveDTO Update(ObjectiveDTO item);
        Task<ObjectiveDTO> UpdateAsync(ObjectiveDTO item);
        void Dispose();
    }
}
