using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.BLL.DTO;
using ProjectManager.BLL.Interfaces;
using ProjectManager.WEB.ViewModels.EntityViewModel;

namespace ProjectManager.WEB.Controllers
{
    public class ObjectiveController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IObjectiveService _objectiveService;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public ObjectiveController(IProjectService projectService, IEmployeeService employeeService, IObjectiveService objectiveService, IMapper mapper)
        {
            _projectService = projectService;
            _objectiveService = objectiveService;
            _employeeService = employeeService;
            _mapper = mapper;
        }
        [Authorize]
        public async Task<IActionResult> Index(Guid id)
        {
            return View(_mapper.Map<ObjectiveViewModel>(await _objectiveService.GetAsync(x => x.Id == id)));
        }

        [Authorize(Roles = "TeamLead, Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _objectiveService.DeleteAsync(id);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "TeamLead, Admin")]
        public async Task<IActionResult> EditObjective(Guid id)
        {
            var objective = await _objectiveService.GetAsync(x => x.Id == id);
            ViewBag.Employees = _mapper.Map<ICollection<EmployeeViewModel>>(_projectService.GetAsync(x => x.Id == objective.ProjectId).Result.Employees);
            return View(_mapper.Map<ObjectiveViewModel>(objective));
        }
        [HttpPost]
        [Authorize(Roles = "TeamLead, Admin")]
        public async Task<IActionResult> EditObjective(ObjectiveViewModel objectiveVM, Guid[] e)
        {
            if (objectiveVM.Id == default)
            {
                objectiveVM.Id = Guid.NewGuid();
            }
            ICollection<EmployeeViewModel> employees = new List<EmployeeViewModel>();
            foreach (var employeeId in e)
            {
                employees.Add(new EmployeeViewModel() { Id = employeeId });
            }
            objectiveVM.Employees = employees;
            await _objectiveService.UpdateAsync(_mapper.Map<ObjectiveDTO>(objectiveVM));
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "TeamLead, Admin")]
        public async Task<IActionResult> AddObjective(Guid id)
        {
            ViewBag.Employees = _mapper.Map<ICollection<EmployeeViewModel>>((await _projectService.GetAsync(x => x.Id == id))?.Employees);
            return View(new ObjectiveViewModel() { ProjectId = id });
        }
        [HttpPost]
        [Authorize(Roles = "TeamLead, Admin")]
        public async Task<IActionResult> AddObjective(ObjectiveViewModel objectiveVM, Guid[] e)
        {
            if (objectiveVM.Id == default)
            {
                objectiveVM.Id = Guid.NewGuid();
            }
            ICollection<EmployeeViewModel> employees = new List<EmployeeViewModel>();
            foreach (var employeeId in e)
            {
                employees.Add(new EmployeeViewModel() { Id = employeeId });
            }
            objectiveVM.Employees = employees;
            var obj = _mapper.Map<ObjectiveDTO>(objectiveVM);
            await _objectiveService.AddAsync(obj);
            return RedirectToAction("Index", "Home");
        }
    }
}
