using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.BLL.DTO;
using ProjectManager.BLL.Interfaces;
using ProjectManager.WEB.ViewModels.EntityViewModel;
using System.Data;

namespace ProjectManager.WEB.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public ProjectController(IProjectService projectService, IEmployeeService employeeService, IMapper mapper)
        {
            _projectService = projectService;
            _employeeService = employeeService;
            _mapper = mapper;
        }


        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Projects = _mapper.Map<ICollection<ProjectViewModel>>(_projectService.GetAll().OrderBy(x => x.Name));
            return View();
        }
        [HttpPost]
        public IActionResult Index(string? searchString, string sortProject)
        {
            if (searchString == null) searchString = "";

            switch (sortProject)
            {
                case "Start":
                    ViewBag.Projects = _mapper.Map<ICollection<ProjectViewModel>>(_projectService.GetAll().Where(x => x.Name.ToLower().Contains(searchString)).OrderBy(x => x.Start));
                    break;
                case "Name":
                    ViewBag.Projects = _mapper.Map<ICollection<ProjectViewModel>>(_projectService.GetAll().Where(x => x.Name.ToLower().Contains(searchString)).OrderBy(x => x.Name));
                    break;
                case "Priority":
                    ViewBag.Projects = _mapper.Map<ICollection<ProjectViewModel>>(_projectService.GetAll().Where(x => x.Name.ToLower().Contains(searchString)).OrderBy(x => x.Priority));
                    break;
                default:
                    ViewBag.Projects = _mapper.Map<ICollection<ProjectViewModel>>(_projectService.GetAll().Where(x => x.Name.ToLower().Contains(searchString)).OrderBy(x => x.Name));
                    break;
            }
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ProjectPage(Guid id)
        {
            ViewBag.Employees = _mapper.Map<ICollection<EmployeeViewModel>>((await _projectService.GetAsync(x => x.Id == id))?.Employees);
            ProjectViewModel project = _mapper.Map<ProjectViewModel>(await _projectService.GetAsync(x => x.Id == id));
            return View(project);
        }

        [Authorize(Roles = "TeamLead, Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _projectService.DeleteAsync(id);
            return RedirectToAction("Index");
        }



        [Authorize(Roles = "TeamLead, Admin")]
        [HttpGet]
        public async Task<IActionResult> EditProject(Guid id)
        {
            ViewBag.Employees = _mapper.Map<ICollection<EmployeeViewModel>>(_employeeService.GetAll());
            return View(_mapper.Map<ProjectViewModel>(await _projectService.GetAsync(x => x.Id == id)));
        }
        [HttpPost]
        [Authorize(Roles = "TeamLead, Admin")]
        public async Task<IActionResult> EditProject(ProjectViewModel projectVM, Guid[] e)
        {
            if (projectVM.Id == default)
            {
                projectVM.Id = Guid.NewGuid();
            }
            ICollection<EmployeeViewModel> employees = new List<EmployeeViewModel>();
            foreach (var employeeId in e)
            {
                employees.Add(new EmployeeViewModel() { Id = employeeId });
            }
            projectVM.Employees = employees;
            var proj = _mapper.Map<ProjectDTO>(projectVM);
            await _projectService.UpdateAsync(proj);
            return RedirectToAction("Index");
        }




        [HttpGet]
        [Authorize(Roles = "TeamLead, Admin")]
        public IActionResult AddProject()
        {
            ViewBag.Employees = _mapper.Map<ICollection<EmployeeViewModel>>(_employeeService.GetAll());
            return View(new ProjectViewModel());
        }
        [HttpPost]
        [Authorize(Roles = "TeamLead, Admin")]
        public async Task<IActionResult> AddProject(ProjectViewModel projectVM, Guid[] e)
        {
            if (projectVM.Id == default)
            {
                projectVM.Id = Guid.NewGuid();
                projectVM.End = default;
            }
            ICollection<EmployeeViewModel> employees = new List<EmployeeViewModel>();
            foreach (var employeeId in e)
            {
                employees.Add(new EmployeeViewModel() { Id = employeeId });
            }
            projectVM.Employees = employees;
            var proj = _mapper.Map<ProjectDTO>(projectVM);
            await _projectService.AddAsync(proj);
            return RedirectToAction("Index");
        }

    }
}

