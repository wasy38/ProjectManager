using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ProjectManager.BLL.DTO;
using ProjectManager.BLL.Interfaces;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces;
using System.Linq.Expressions;

namespace ProjectManager.BLL.Services
{
    public class EmployeeService : IEmployeeService
    {
        private IUnitOfWork _uow { get; set; }
        private IMapper _mapper { get; set; }
        private UserManager<Employee> _userManager { get; set; }
        private RoleManager<IdentityRole<Guid>> _roleManager { get; set; }

        public EmployeeService(IUnitOfWork uow,
            IMapper mapper,
            UserManager<Employee> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            _uow = uow;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Add(EmployeeDTO item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            Employee employee = _mapper.Map<Employee>(item);
            _uow.GetRepository<Employee>().Add(employee);
            _uow.Save();
        }

        public async Task<EmployeeDTO> AddAsync(EmployeeDTO item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            Employee employee = _mapper.Map<Employee>(item);
            await _uow.GetRepository<Employee>().AddAsync(employee);
            await _uow.SaveChangesAsync();
            return item;
        }

        public void Delete(Guid id)
        {
            Employee employee = _uow.GetRepository<Employee>().Get(x => x.Id == id);
            if (employee != null)
            {
                _uow.GetRepository<Employee>().Delete(employee);
                _uow.Save();
            }
            else throw new ArgumentException(nameof(id));
        }

        public async Task DeleteAsync(Guid id)
        {
            Employee employee = await _uow.GetRepository<Employee>().GetAsync(x => x.Id == id);
            if (employee != null)
            {
                await _uow.GetRepository<Employee>().DeleteAsync(employee);
                await _uow.SaveChangesAsync();
            }
            else throw new ArgumentException(nameof(id));
        }

        public void Dispose()
        {
            _uow.Dispose();
        }

        public EmployeeDTO Get(Expression<Func<Employee, bool>>? predicate)
        {
            return _mapper.Map<EmployeeDTO>(_uow.GetRepository<Employee>().Get(predicate));
        }

        public ICollection<EmployeeDTO> GetAll()
        {
            return _mapper.Map<ICollection<EmployeeDTO>>(_uow.GetRepository<Employee>().GetAll());
        }

        public async Task<EmployeeDTO> GetAsync(Expression<Func<Employee, bool>>? predicate)
        {
            Employee employee = await _uow.GetRepository<Employee>().GetAsync(predicate);
            return _mapper.Map<EmployeeDTO>(employee);
        }


        public async Task<EmployeeDTO> UpdateAsync(EmployeeDTO item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            var user = await _userManager.FindByIdAsync(item.Id.ToString());
            if (item.Roles != null)
            {
                var roles = new List<string>();
                foreach (var role in item.Roles)
                {
                    roles.Add(_roleManager.FindByIdAsync(role.ToString()).Result.ToString());
                }
                var Roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, Roles);
                await _userManager.AddToRolesAsync(user, roles);
            }

            if (item.Password != null)
                await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), item.Password);


            if (user.Email != item.Email)
            {
                await _userManager.ChangeEmailAsync(user, item.Email, await _userManager.GenerateChangeEmailTokenAsync(user, item.Email));
                await _userManager.SetUserNameAsync(user, item.Email);
            }

            await _uow.SaveChangesAsync();
            return item;
        }
    }
}
