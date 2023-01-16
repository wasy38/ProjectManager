using ProjectManager.BLL.DTO;
using ProjectManager.Domain.Entities;
using System.Linq.Expressions;

namespace ProjectManager.BLL.Interfaces
{
    public interface IEmployeeService
    {
        void Add(EmployeeDTO item);
        Task<EmployeeDTO> AddAsync(EmployeeDTO item);
        EmployeeDTO Get(Expression<Func<Employee, bool>>? predicate);
        Task<EmployeeDTO> GetAsync(Expression<Func<Employee, bool>>? predicate);
        ICollection<EmployeeDTO> GetAll();
        void Delete(Guid id);
        Task DeleteAsync(Guid id);
        Task<EmployeeDTO> UpdateAsync(EmployeeDTO item);
        void Dispose();
    }
}
