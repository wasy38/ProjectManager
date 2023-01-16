using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.BLL.DTO;
using ProjectManager.BLL.Interfaces;
using ProjectManager.Domain.Entities;
using ProjectManager.WEB.ViewModels.EntityViewModel;
using System.Data;

namespace ProjectManager.WEB.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public EmployeeController(IEmployeeService employeeService, IMapper mapper, UserManager<Employee> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Employees = _mapper.Map<ICollection<EmployeeViewModel>>(_employeeService.GetAll());
            return View();
        }
        [Authorize]
        [HttpPost]
        public IActionResult Index(string? searchString)
        {
            if (searchString == null) searchString = "";
            var emp = from e in _employeeService.GetAll() where e.FName.ToLower().Contains(searchString) || e.SName.ToLower().Contains(searchString) || e.Patronymic.ToLower().Contains(searchString) select e;
            ViewBag.Employees = _mapper.Map<ICollection<EmployeeViewModel>>(emp);
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _employeeService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditEmployee(Guid id)
        {
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View(_mapper.Map<EmployeeViewModel>(await _employeeService.GetAsync(x => x.Id == id)));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditEmployee(EmployeeViewModel employeeVM, Guid[] roles)
        {
            employeeVM.Roles = roles;
            await _employeeService.UpdateAsync(_mapper.Map<EmployeeDTO>(employeeVM));
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangePassword(Guid id)
        {
            return View(_mapper.Map<EmployeeViewModel>(await _employeeService.GetAsync(x => x.Id == id)));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangePassword(EmployeeViewModel employeeViewModel)
        {
            await EditEmployee(employeeViewModel, null);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EmployeePage(Guid id)
        {
            EmployeeViewModel employee = _mapper.Map<EmployeeViewModel>(await _employeeService.GetAsync(x => x.Id.Equals(id)));
            return View(employee);
        }
    }
}
