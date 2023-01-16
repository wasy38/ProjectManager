using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectManager.BLL.DTO;
using ProjectManager.BLL.Interfaces;
using ProjectManager.Domain.Entities;
using ProjectManager.WEB.AutoMapperProfiles;
using ProjectManager.WEB.Controllers;
using ProjectManager.WEB.ViewModels.EntityViewModel;
using System.Linq.Expressions;

namespace ProjectManager.Tests.Controllers
{
    public class ProjectControllerTest
    {
        #region reference data
        static private Guid[] _guids = new Guid[5] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        static private Mock<IProjectService> projMock = new Mock<IProjectService>();
        static private Mock<IEmployeeService> empMock = new Mock<IEmployeeService>();
        static private MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> { new ProjectProfile(), new ObjectiveProfile(), new EmployeeProfile() }));
        static private Mapper mapper = new Mapper(config);

        static private ProjectDTO newProject = new ProjectDTO { CustomerName = "qwe", Name = "qwe", Priority = 132, PerformerName = "qwe", Id = Guid.NewGuid() };

        private List<EmployeeDTO> GetTestEmployees()
        {
            var employees = new List<EmployeeDTO>
            {
                new EmployeeDTO
                {
                    Id=_guids[4],
                    Email="1@1",
                    FName="1",
                    Password="password",
                    Patronymic="1",
                    SName="2",
                }
            };
            return employees;
        }

        private List<ProjectDTO> GetTestProjects()
        {
            var projects = new List<ProjectDTO>
            {
                new ProjectDTO
                {
                    Id=_guids[1],
                    Name = "Test1",
                    CustomerName = "Test1",
                    PerformerName = "Test1",
                    Priority = 100,
                    Objectives = new List<ObjectiveDTO>(),
                    Employees = new List<EmployeeDTO>()
                },
                new ProjectDTO
                {
                    Id = _guids[0],
                    Name = "Test2",
                    CustomerName = "Test1",
                    PerformerName = "Test1",
                    Priority = 100,
                    Objectives = new List<ObjectiveDTO>(),
                    Employees = new List<EmployeeDTO>()
                },
                new ProjectDTO
                {
                    Id = _guids[2],
                    Name = "111",
                    CustomerName = "Test1",
                    PerformerName = "Test1",
                    Priority = 100,
                    Objectives = new List<ObjectiveDTO>(),
                    Employees = new List<EmployeeDTO>()
                }
            };
            return projects;
        }

        private ProjectController Init()
        {
            projMock.Setup(projserv => projserv.GetAll()).Returns(GetTestProjects());
            projMock.Setup(projserv => projserv.GetAsync(It.IsAny<Expression<Func<Project, bool>>>()).Result).Returns(GetTestProjects().FirstOrDefault(x => x.Id == _guids[1]));
            empMock.Setup(empServ => empServ.GetAll()).Returns(GetTestEmployees());
            projMock.Setup(projserv => projserv.UpdateAsync(It.IsAny<ProjectDTO>()).Result).Returns(GetTestProjects().FirstOrDefault(x => x.Id == _guids[1]));

            projMock.Setup(projserv => projserv.AddAsync(It.IsAny<ProjectDTO>()).Result).Returns(newProject);

            ProjectController controller = new ProjectController(projMock.Object, empMock.Object, mapper);
            return controller;
        }
        #endregion

        [Fact]
        public void IndexViewReturnViewResult()
        {
            // Arrange
            var controller = Init();
            // Act
            ViewResult result = controller.Index() as ViewResult;
            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void IndexReturnsAViewResultWithAListOfProjects()
        {
            // Arrange
            var controller = Init();
            // Act
            ViewResult result = controller.Index() as ViewResult;
            var vdDTO = mapper.Map<ICollection<ProjectDTO>>(result.ViewData["Projects"]).ToList();
            var testProjects = GetTestProjects();
            bool assertResult = false;
            if (testProjects.Count == vdDTO.Count)
            {
                assertResult = true;
                for (int i = 0; i < testProjects.Count; i++)
                {
                    if (testProjects.FirstOrDefault(x => x.Id == vdDTO[i].Id) == null)
                        assertResult = false;
                }
            }
            // Assert
            Assert.True(assertResult);
        }

        [Fact]
        public void IndexReturnsAViewResultWithASortedFoundListOfProjects()
        {
            // Arrange
            var controller = Init();
            // Act
            ViewResult result = controller.Index("1", "Name") as ViewResult;
            var vdDTO = mapper.Map<ICollection<ProjectDTO>>(result.ViewData["Projects"]).ToList();
            List<ProjectDTO> sortedProjects = GetTestProjects().Where(x => x.Name.ToLower().Contains("1")).OrderBy(x => x.Name).ToList();
            bool assertResult = false;
            if (sortedProjects.Count == vdDTO.Count)
            {
                assertResult = true;
                for (int i = 0; i < sortedProjects.Count; i++)
                {
                    if (sortedProjects[i].Id != vdDTO[i].Id)
                    {
                        assertResult = false;
                    }
                }
            }
            // Assert
            Assert.True(assertResult);
        }

        [Fact]
        public void ProjectPageViewReturnViewResult()
        {
            // Arrange
            var controller = Init();
            // Act
            ViewResult result = controller.ProjectPage(_guids[1]).Result as ViewResult;
            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ProjectPageReturnsAViewResultPage()
        {
            // Arrange
            var controller = Init();
            // Act
            var result = controller.ProjectPage(_guids[1]).Result as ViewResult;
            var model = result.ViewData.Model as ProjectViewModel;
            var vdDTO = mapper.Map<ICollection<EmployeeDTO>>(result.ViewData["Employees"]).ToList();
            var testProject = GetTestProjects().FirstOrDefault(x => x.Id == _guids[1]);
            bool assertResult = false;
            if (testProject.Employees.Count == vdDTO.Count)
            {
                assertResult = true;
                if (testProject.Employees.FirstOrDefault() != null)
                    for (int i = 0; i < testProject.Employees.Count; i++)
                    {
                        if (testProject.Employees.FirstOrDefault(x => x.Id == vdDTO[i].Id) == null)
                            assertResult = false;
                    }
            }
            if ((testProject.Id != model.Id) || !assertResult)
                assertResult = false;
            // Assert
            Assert.True(assertResult);
        }

        [Fact]
        public void DeleteProjectViewReturnViewResulProjects()
        {
            // Arrange
            var controller = Init();
            // Act
            var result = controller.Delete(_guids[0]).Result;
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            projMock.Verify(u => u.DeleteAsync(_guids[0]));
        }

        [Fact]
        public void EditProjectViewReturnViewResult()
        {
            // Arrange
            var controller = Init();
            // Act
            ViewResult result = controller.EditProject(_guids[1]).Result as ViewResult;
            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void EditProjectReturnsAViewResultPage()
        {
            // Arrange
            var testEmployees = GetTestEmployees();
            var controller = Init();
            // Act
            var result = controller.EditProject(_guids[1]).Result as ViewResult;
            var model = result.ViewData.Model as ProjectViewModel;
            var viewData = result.ViewData["Employees"];
            var vdDTO = mapper.Map<ICollection<EmployeeDTO>>(viewData).ToList();
            var testProject = GetTestProjects().FirstOrDefault(x => x.Id == _guids[1]);
            bool assertResult = false;
            if (testEmployees.Count == vdDTO.Count)
            {
                assertResult = true;
                if (testEmployees != null)
                    for (int i = 0; i < testEmployees.Count; i++)
                    {
                        if (testEmployees.FirstOrDefault(x => x.Id == vdDTO[i].Id) == null)
                            assertResult = false;
                    }
            }
            if ((testProject.Id != model.Id) || !assertResult)
                assertResult = false;
            // Assert
            Assert.True(assertResult);
        }

        [Fact]
        public void EditProjectReturnsAModifiedProjectAndRedirectToIndex()
        {
            // Arrange
            ProjectDTO sellectdProject = GetTestProjects().FirstOrDefault(x => x.Id == _guids[1]);
            ProjectViewModel editProj = mapper.Map<ProjectViewModel>(sellectdProject);
            var controller = Init();

            // Act
            var result = controller.EditProject(editProj, new Guid[] { }).Result;

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            projMock.Verify(u => u.UpdateAsync(It.IsAny<ProjectDTO>()));
        }

        [Fact]
        public void AddPageViewReturnViewResult()
        {
            // Arrange
            var controller = Init();
            // Act
            ViewResult result = controller.AddProject() as ViewResult;
            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void AddProjectReturnsAViewResultPage()
        {
            // Arrange
            var testEmployees = GetTestEmployees();
            var controller = Init();
            // Act
            var result = controller.AddProject() as ViewResult;
            var viewData = result.ViewData["Employees"];
            var vdDTO = mapper.Map<ICollection<EmployeeDTO>>(viewData).ToList();
            bool assertResult = false;
            if (testEmployees.Count == vdDTO.Count)
            {
                assertResult = true;
                if (testEmployees != null)
                    for (int i = 0; i < testEmployees.Count; i++)
                    {
                        if (testEmployees.FirstOrDefault(x => x.Id == vdDTO[i].Id) == null)
                            assertResult = false;
                    }
            }
            // Assert
            Assert.True(assertResult);
        }

        [Fact]
        public void AddProjectReturnsAddsProjectAndRedirectToIndex()
        {
            // Arrange
            ProjectViewModel viewModelProj = mapper.Map<ProjectViewModel>(newProject);
            var controller = Init();
            // Act
            var result = controller.AddProject(viewModelProj, new Guid[] { }).Result;
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            projMock.Verify(u => u.AddAsync(It.IsAny<ProjectDTO>()));
        }
    }
}
