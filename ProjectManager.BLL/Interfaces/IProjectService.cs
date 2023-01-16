using ProjectManager.BLL.DTO;
using ProjectManager.Domain.Entities;
using System.Linq.Expressions;

namespace ProjectManager.BLL.Interfaces
{
    public interface IProjectService
    {
        void Add(ProjectDTO item);
        Task<ProjectDTO> AddAsync(ProjectDTO item);
        ProjectDTO Get(Expression<Func<Project, bool>>? predicate);
        Task<ProjectDTO> GetAsync(Expression<Func<Project, bool>>? predicate);
        ICollection<ProjectDTO> GetAll();
        void Delete(Guid id);
        Task DeleteAsync(Guid id);
        ProjectDTO Update(ProjectDTO item);
        Task<ProjectDTO> UpdateAsync(ProjectDTO item);
        void Dispose();
    }
}
