using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
    public class EmployeeControllerTest
    {
        #region reference data
        private static Guid[] _guids = new Guid[5] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        private static MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> { new ProjectProfile(), new ObjectiveProfile(), new EmployeeProfile() }));
        private static Mapper mapper = new Mapper(config);
        private static Mock<IEmployeeService> empMock = new Mock<IEmployeeService>();
        private static Mock<UserManager<Employee>> userMock = GetMockUserManager();
        private static Mock<RoleManager<IdentityRole<Guid>>> roleMock = GetMockRoleManager();
        private static IQueryable<IdentityRole<Guid>> testRoles = new List<IdentityRole<Guid>> { new IdentityRole<Guid> { Name = "Adm", Id = Guid.NewGuid(), NormalizedName = "ADM" } }.AsQueryable();

        private static Mock<UserManager<Employee>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<Employee>>();
            return new Mock<UserManager<Employee>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        }
        private static Mock<RoleManager<IdentityRole<Guid>>> GetMockRoleManager()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole<Guid>>>();
            return new Mock<RoleManager<IdentityRole<Guid>>>(roleStore.Object, null, null, null, null);

        }
        private List<EmployeeDTO> GetTestEmployees()
        {
            var employees = new List<EmployeeDTO>
            {
                new EmployeeDTO
                {
                    Id=_guids[0],
                    Email="1@1",
                    FName="1",
                    Password="password",
                    Patronymic="1",
                    SName="2",
                },
                new EmployeeDTO
                {
                    Id=_guids[1],
                    Email="2@2",
                    FName="2",
                    Password="password",
                    Patronymic="2",
                    SName="1",
                }

            };
            return employees;
        }

        private EmployeeController Init()
        {
            empMock.Setup(employeeService => employeeService.GetAll()).Returns(GetTestEmployees());
            empMock.Setup(employeeService => employeeService.GetAsync(It.IsAny<Expression<Func<Employee, bool>>>()).Result).Returns(GetTestEmployees().FirstOrDefault(x => x.Id == _guids[0]));
            roleMock.Setup(roleManager => roleManager.Roles).Returns(testRoles);

            EmployeeController controller = new EmployeeController(empMock.Object, mapper, userMock.Object, roleMock.Object);
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
        public void IndexReturnsAViewResultWithAListOfEmployees()
        {
            // Arrange
            var testEmployees = GetTestEmployees();
            var controller = Init();
            // Act
            ViewResult result = controller.Index() as ViewResult;
            var vdDTO = mapper.Map<ICollection<EmployeeDTO>>(result.ViewData["Employees"]).ToList();
            bool assertResult = false;
            if (testEmployees.Count == vdDTO.Count)
            {
                assertResult = true;
                foreach (var employeeViewModel in vdDTO)
                {
                    if (testEmployees.FirstOrDefault(x => x.Id == employeeViewModel.Id) == null)
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
            ViewResult result = controller.Index("1") as ViewResult;
            var vdDTO = mapper.Map<ICollection<EmployeeDTO>>(result.ViewData["Employees"]).ToList();
            List<EmployeeDTO> sortedEmployees = GetTestEmployees().Where(e => e.FName.ToLower().Contains("1") || e.SName.ToLower().Contains("1") || e.Patronymic.ToLower().Contains("1")).ToList();
            bool assertResult = false;
            if (sortedEmployees.Count == vdDTO.Count)
            {
                assertResult = true;
                for (int i = 0; i < sortedEmployees.Count; i++)
                {
                    if (sortedEmployees[i].Id != vdDTO[i].Id)
                    {
                        assertResult = false;
                    }
                }
            }
            // Assert
            Assert.True(assertResult);
        }

        [Fact]
        public void EmployeePageViewReturnViewResult()
        {
            // Arrange
            var testEmployees = GetTestEmployees();
            var sellected = GetTestEmployees().FirstOrDefault(x => x.Id == _guids[0]);
            var controller = Init();
            // Act
            ViewResult result = controller.EmployeePage(_guids[0]).Result as ViewResult;
            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void EmployeePageReturnsAViewResultPage()
        {
            // Arrange
            var testEmployees = GetTestEmployees();
            var sellected = GetTestEmployees().FirstOrDefault(x => x.Id == _guids[0]);
            var controller = Init();
            // Act
            var result = controller.EmployeePage(_guids[0]).Result as ViewResult;
            var model = result.ViewData.Model as EmployeeViewModel;
            bool assertResult = false;
            if (sellected.Id == model.Id)
                assertResult = true;
            // Assert
            Assert.True(assertResult);
        }

        [Fact]
        public void DeleteEmployeeViewReturnViewResulEmployees()
        {
            // Arrange
            var controller = Init();
            // Act
            var result = controller.Delete(_guids[1]).Result;
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            empMock.Verify(x => x.DeleteAsync(_guids[1]));
        }

        [Fact]
        public void EditEmployeeViewReturnViewResult()
        {
            // Arrange
            var testEmployee = GetTestEmployees().FirstOrDefault(x => x.Id == _guids[0]);
            var controller = Init();
            // Act
            ViewResult result = controller.EditEmployee(_guids[0]).Result as ViewResult;
            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void EditEmployeeReturnsAViewResultPage()
        {
            // Arrange
            var testEmployee = GetTestEmployees().FirstOrDefault(x => x.Id == _guids[0]);
            var controller = Init();
            // Act
            var result = controller.EditEmployee(_guids[0]).Result as ViewResult;
            var model = result.ViewData.Model as EmployeeViewModel;
            var viewData = result.ViewData["Roles"] as List<IdentityRole<Guid>>;

            bool assertResult = false;
            if ((testEmployee.Id == model.Id) && (testRoles.FirstOrDefault(x => x.Name == "Adm") == viewData.FirstOrDefault(x => x.Name == "Adm")))
                assertResult = true;
            // Assert
            Assert.True(assertResult);
        }

        [Fact]
        public void EditEmployeeReturnsAModifiedEmployeeAndRedirectToIndex()
        {
            // Arrange
            var testEmployee = GetTestEmployees().FirstOrDefault(x => x.Id == _guids[0]);
            var employeeVM = mapper.Map<EmployeeViewModel>(testEmployee);
            testEmployee.Roles = new Guid[] { _guids[3] };
            var controller = Init();
            // Act
            var result = controller.EditEmployee(employeeVM, testEmployee.Roles).Result;
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            empMock.Verify(u => u.UpdateAsync(It.IsAny<EmployeeDTO>()));
        }

        [Fact]
        public void ChangePasswordViewReturnViewResult()
        {
            // Arrange
            var testEmployee = GetTestEmployees().FirstOrDefault(x => x.Id == _guids[0]);
            var controller = Init();
            // Act
            ViewResult result = controller.ChangePassword(_guids[0]).Result as ViewResult;
            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ChangePasswordReturnsAViewResultPage()
        {
            // Arrange
            var testEmployee = GetTestEmployees().FirstOrDefault(x => x.Id == _guids[0]);
            var controller = Init();
            // Act
            var result = controller.ChangePassword(_guids[0]).Result as ViewResult;
            var model = result.ViewData.Model as EmployeeViewModel;
            // Assert
            Assert.Equal(testEmployee.Id, model.Id);
        }

        [Fact]
        public void ChangePasswordReturnsAModifiedEmployeeAndRedirectToIndex()
        {
            // Arrange
            var testEmployee = GetTestEmployees().FirstOrDefault(x => x.Id == _guids[0]);
            var employeeVM = mapper.Map<EmployeeViewModel>(testEmployee);
            var controller = Init();
            // Act
            var result = controller.ChangePassword(employeeVM).Result;
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}
