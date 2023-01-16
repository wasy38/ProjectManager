using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManager.BLL.DTO;
using ProjectManager.BLL.Interfaces;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces;
using System.Linq.Expressions;

namespace ProjectManager.BLL.Services
{
    public class ProjectService : IProjectService
    {

        private IUnitOfWork _uow { get; set; }
        private IMapper _mapper { get; set; }

        public ProjectService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        public void Add(ProjectDTO item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            Project project = _mapper.Map<Project>(item);
            _uow.GetRepository<Project>().Add(project);
            _uow.Save();
        }

        public async Task<ProjectDTO> AddAsync(ProjectDTO item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            Project project = _mapper.Map<Project>(item);
            ICollection<Employee> ListOfEmployees = new List<Employee>();
            foreach (var employeeDTO in item.Employees)
            {
                var employee = await _uow.GetRepository<Employee>().GetAsync(x => x.Id == employeeDTO.Id);
                if (employee != null)
                {
                    ListOfEmployees.Add(employee);
                }
            }
            project.Employees = ListOfEmployees;
            await _uow.GetRepository<Project>().AddAsync(project);
            await _uow.SaveChangesAsync();
            return item;
        }

        public void Delete(Guid id)
        {
            Project project = _uow.GetRepository<Project>().Get(x => x.Id == id);
            if (project != null)
            {
                _uow.GetRepository<Project>().Delete(project);
                _uow.Save();
            }
            else throw new ArgumentException(nameof(id));
        }

        public async Task DeleteAsync(Guid id)
        {
            Project project = await _uow.GetRepository<Project>().GetAsync(x => x.Id == id);
            if (project != null)
            {
                await _uow.GetRepository<Project>().DeleteAsync(project);
                await _uow.SaveChangesAsync();
            }
            else throw new ArgumentException(nameof(id));
        }

        public void Dispose()
        {
            _uow.Dispose();
        }

        public ProjectDTO Get(Expression<Func<Project, bool>>? predicate)
        {
            return _mapper.Map<ProjectDTO>(_uow.GetRepository<Project>().Get(predicate));
        }

        public ICollection<ProjectDTO> GetAll()
        {
            ICollection<Project> a = _uow.GetRepository<Project>().GetAll().Include(x => x.Employees).ToList();
            return _mapper.Map<ICollection<ProjectDTO>>(a);
        }

        public async Task<ProjectDTO> GetAsync(Expression<Func<Project, bool>>? predicate)
        {
            Project project = await _uow.GetRepository<Project>().GetAsync(predicate, x => x.Employees, x => x.Objectives);
            return _mapper.Map<ProjectDTO>(project);
        }

        public ProjectDTO Update(ProjectDTO item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            Project project = _uow.GetRepository<Project>().Get(x => x.Id == item.Id);
            if (project == null)
            {
                project = _mapper.Map<Project>(item);
            }

            _mapper.Map(item, project);

            project = _uow.GetRepository<Project>().Update(project);
            _uow.Save();
            return _mapper.Map<ProjectDTO>(project);
        }


        public async Task<ProjectDTO> UpdateAsync(ProjectDTO item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            Project project = await _uow.GetRepository<Project>().GetAsync(x => x.Id == item.Id, x => x.Employees);
            if (project == null)
            {
                project = _mapper.Map<Project>(item);
            }


            ICollection<Employee> ListOfEmployees = new List<Employee>();
            foreach (var employeeDTO in item.Employees)
            {
                var employee = await _uow.GetRepository<Employee>().GetAsync(x => x.Id == employeeDTO.Id);
                if (employee != null)
                {
                    ListOfEmployees.Add(employee);
                }
            }

            _mapper.Map(item, project);

            project.Employees = ListOfEmployees;

            project = await _uow.GetRepository<Project>().UpdateAsync(project);
            await _uow.SaveChangesAsync();
            return _mapper.Map<ProjectDTO>(project);
        }
    }
}
