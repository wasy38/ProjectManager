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
    public class ObjectiveControllerTest
    {
        #region reference data
        private static Guid[] _guids = new Guid[5] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        private static Mock<IProjectService> projMock = new Mock<IProjectService>();
        private static Mock<IEmployeeService> empMock = new Mock<IEmployeeService>();
        private static new Mock<IObjectiveService> objMock = new Mock<IObjectiveService>();
        private static MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> { new ProjectProfile(), new ObjectiveProfile(), new EmployeeProfile() }));
        private static Mapper mapper = new Mapper(config);

        private static ProjectDTO testProject = new ProjectDTO()
        {
            Id = _guids[4],
            Name = "PROJECT",
            Employees = new List<EmployeeDTO>()
            {
                new EmployeeDTO { Id = _guids[3] }
            }
        };
        private static ObjectiveDTO newObjective = new ObjectiveDTO
        {
            Id = Guid.NewGuid(),
            Author = "123",
            Comment = "Com",
            Name = "name",
            Priority = 1,
            Project = testProject,
            ProjectId = testProject.Id,
            Status = Domain.Enums.Status.InProgress
        };

        private List<ObjectiveDTO> GetTestObjectives()
        {
            var objectives = new List<ObjectiveDTO>
            {
                new ObjectiveDTO
                {
                    Id=_guids[1],
                    Name = "Test1",
                    Author = "13",
                    Comment = "123",
                    Priority = 123,
                    Status = Domain.Enums.Status.ToDo,
                    Employees = new List<EmployeeDTO>(),
                    Project = testProject,
                    ProjectId = _guids[1]
                },
                new ObjectiveDTO
                {
                    Id = _guids[0],
                    Name = "Test2",
                    Author = "123",
                    Comment = "COMMENT",
                    Project = testProject,
                    ProjectId = _guids[1],
                    Status = Domain.Enums.Status.InProgress,
                    Priority = 100,
                    Employees = new List<EmployeeDTO>()
                },
            };
            return objectives;
        }

        ObjectiveController Init()
        {
            objMock.Setup(objserv => objserv.GetAsync(It.IsAny<Expression<Func<Objective, bool>>>()).Result).Returns(GetTestObjectives().FirstOrDefault(x => x.Id == _guids[1]));
            projMock.Setup(projserv => projserv.GetAsync(It.IsAny<Expression<Func<Project, bool>>>()).Result).Returns(testProject);
            objMock.Setup(objserv => objserv.AddAsync(It.IsAny<ObjectiveDTO>()).Result).Returns(newObjective);
            objMock.Setup(objeciveService => objeciveService.UpdateAsync(It.IsAny<ObjectiveDTO>()).Result).Returns(GetTestObjectives().FirstOrDefault(x => x.Id == _guids[1]));

            ObjectiveController controller = new ObjectiveController(projMock.Object, empMock.Object, objMock.Object, mapper);
            return controller;
        }
        #endregion

        [Fact]
        public void IndexViewReturnViewResult()
        {
            // Arrange
            var controller = Init();
            // Act
            ViewResult result = controller.Index(_guids[1]).Result as ViewResult;
            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void IndexReturnsAViewResultWithAListOfObjectives()
        {
            // Arrange
            var sellectedObjective = GetTestObjectives().FirstOrDefault(x => x.Id == _guids[1]);
            var controller = Init();
            // Act
            ViewResult result = controller.Index(_guids[1]).Result as ViewResult;
            var model = result.ViewData.Model as ObjectiveViewModel;

            bool assertResult = false;
            if (model.Id == sellectedObjective.Id)
                assertResult = true;
            // Assert
            Assert.True(assertResult);
        }

        [Fact]
        public void DeleteObjectiveViewReturnViewResulProjects()
        {
            // Arrange
            var controller = Init();
            // Act
            var result = controller.Delete(_guids[1]).Result;
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            objMock.Verify(x => x.DeleteAsync(_guids[1]));
        }

        [Fact]
        public void AddObjectiveViewReturnViewResult()
        {
            // Arrange
            var controller = Init();
            // Act
            ViewResult result = controller.AddObjective(_guids[4]).Result as ViewResult;
            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void AddObjectiveReturnsAViewResultPage()
        {
            // Arrange
            var controller = Init();
            // Act
            var result = controller.AddObjective(_guids[4]).Result as ViewResult;
            bool assertResult = false;
            var vbViewModel = result.ViewData["Employees"] as ICollection<EmployeeViewModel>;

            if (vbViewModel.Count == testProject.Employees.Count)
            {
                assertResult = true;
                if (vbViewModel != null)
                {
                    foreach (var employeeViewModel in vbViewModel)
                    {
                        if (testProject.Employees.FirstOrDefault(x => x.Id == employeeViewModel.Id) == null)
                            assertResult = false;
                    }
                }
            }
            // Assert
            Assert.True(assertResult);
        }

        [Fact]
        public void AddObjectiveReturnsAddsObjectiveAndRedirectToIndex()
        {
            // Arrange
            ObjectiveViewModel viewModelObjective = mapper.Map<ObjectiveViewModel>(newObjective);
            var controller = Init();
            // Act
            var result = controller.AddObjective(viewModelObjective, new Guid[] { }).Result;
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            objMock.Verify(u => u.AddAsync(It.IsAny<ObjectiveDTO>()));
        }

        [Fact]
        public void EditObjectiveViewReturnViewResult()
        {
            // Arrange
            var editObjective = GetTestObjectives().FirstOrDefault(x => x.Id == _guids[1]);
            var controller = Init();
            // Act
            ViewResult result = controller.EditObjective(_guids[4]).Result as ViewResult;
            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void EditObjectiveReturnsAViewResultPage()
        {
            // Arrange
            var editObjective = GetTestObjectives().FirstOrDefault(x => x.Id == _guids[1]);
            var controller = Init();
            // Act
            var result = controller.EditObjective(_guids[4]).Result as ViewResult;
            bool assertResult = false;
            var vbViewModel = result.ViewData["Employees"] as ICollection<EmployeeViewModel>;
            var model = result.ViewData.Model as ObjectiveViewModel;

            if (vbViewModel.Count == testProject.Employees.Count)
            {
                assertResult = true;
                if (vbViewModel != null)
                {
                    foreach (var employeeViewModel in vbViewModel)
                    {
                        if (testProject.Employees.FirstOrDefault(x => x.Id == employeeViewModel.Id) == null)
                            assertResult = false;
                    }
                }
            }
            if ((editObjective.Id != model.Id) || !assertResult)
                assertResult = false;
            // Assert
            Assert.True(assertResult);
        }


        [Fact]
        public void EditObjectiveReturnsAModifiedProjectAndRedirectToIndex()
        {
            // Arrange
            var editObjective = GetTestObjectives().FirstOrDefault(x => x.Id == _guids[1]);
            var objectiveViewModel = mapper.Map<ObjectiveViewModel>(editObjective);
            var controller = Init();
            // Act
            var result = controller.EditObjective(objectiveViewModel, new Guid[] { }).Result;
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            objMock.Verify(u => u.UpdateAsync(It.IsAny<ObjectiveDTO>()));
        }
    }
}
