using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManager.BLL.DTO;
using ProjectManager.BLL.Interfaces;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces;
using System.Linq.Expressions;

namespace ProjectManager.BLL.Services
{
    public class ObjectiveService : IObjectiveService
    {
        private IUnitOfWork _uow { get; set; }
        private IMapper _mapper { get; set; }

        public ObjectiveService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        public void Add(ObjectiveDTO item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            Objective objective = _mapper.Map<Objective>(item);
            _uow.GetRepository<Objective>().Add(objective);
            _uow.Save();
        }

        public async Task<ObjectiveDTO> AddAsync(ObjectiveDTO item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            Objective objective = _mapper.Map<Objective>(item);
            ICollection<Employee> ListOfEmployees = new List<Employee>();
            foreach (var employeeDTO in item.Employees)
            {
                var employee = await _uow.GetRepository<Employee>().GetAsync(x => x.Id == employeeDTO.Id);
                if (employee != null)
                {
                    ListOfEmployees.Add(employee);
                }
            }
            var proj = await _uow.GetRepository<Project>().GetAsync(x => x.Id == objective.ProjectId);
            if (proj != null)
            {
                objective.Project = proj;
            }
            objective.Employees = ListOfEmployees;
            await _uow.GetRepository<Objective>().AddAsync(objective);
            await _uow.SaveChangesAsync();
            return item;
        }

        public void Delete(Guid id)
        {
            Objective objective = _uow.GetRepository<Objective>().Get(x => x.Id == id);
            if (objective != null)
            {
                _uow.GetRepository<Objective>().Delete(objective);
                _uow.Save();
            }
            else throw new ArgumentException(nameof(id));
        }

        public async Task DeleteAsync(Guid id)
        {
            Objective objective = await _uow.GetRepository<Objective>().GetAsync(x => x.Id == id);
            if (objective != null)
            {
                await _uow.GetRepository<Objective>().DeleteAsync(objective);
                await _uow.SaveChangesAsync();
            }
        }

        public void Dispose()
        {
            _uow.Dispose();
        }

        public ObjectiveDTO Get(Expression<Func<Objective, bool>>? predicate)
        {
            return _mapper.Map<ObjectiveDTO>(_uow.GetRepository<Objective>().Get(predicate));
        }

        public ICollection<ObjectiveDTO> GetAll()
        {
            var a = _uow.GetRepository<Objective>().GetAll().Include(x => x.Employees);
            return _mapper.Map<ICollection<ObjectiveDTO>>(a);
        }

        public async Task<ObjectiveDTO> GetAsync(Expression<Func<Objective, bool>>? predicate)
        {
            Objective objective = await _uow.GetRepository<Objective>().GetAsync(predicate, x => x.Project, x => x.Employees);
            return _mapper.Map<ObjectiveDTO>(objective);
        }

        public ObjectiveDTO Update(ObjectiveDTO item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            Objective objective = _uow.GetRepository<Objective>().Get(x => x.Id == item.Id);
            if (objective == null)
            {
                objective = _mapper.Map<Objective>(item);
            }

            _mapper.Map(item, objective);

            objective = _uow.GetRepository<Objective>().Update(objective);
            _uow.Save();
            return _mapper.Map<ObjectiveDTO>(objective);
        }

        public async Task<ObjectiveDTO> UpdateAsync(ObjectiveDTO item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            Objective objective = await _uow.GetRepository<Objective>().GetAsync(x => x.Id == item.Id);
            if (objective == null)
            {
                objective = _mapper.Map<Objective>(item);
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

            objective.Employees = ListOfEmployees;

            _mapper.Map(item, objective);

            objective = await _uow.GetRepository<Objective>().UpdateAsync(objective);
            await _uow.SaveChangesAsync();
            return _mapper.Map<ObjectiveDTO>(objective);
        }
    }
}

