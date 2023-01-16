using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using ProjectManager.BLL.DTO;
using ProjectManager.BLL.Services;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces;
using ProjectManager.WEB.AutoMapperProfiles;
using System.Linq.Expressions;

namespace ProjectManager.Tests.Services
{
    public class EmployeeServiceTest
    {
        #region reference data
        private static Guid[] _guids = new Guid[5] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        private static MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> { new ProjectProfile(), new ObjectiveProfile(), new EmployeeProfile() }));
        private static Mapper mapper = new Mapper(config);
        private static Mock<IUnitOfWork> uowMock = new Mock<IUnitOfWork>();
        private static Mock<UserManager<Employee>> userMock = GetMockUserManager();
        private static Mock<RoleManager<IdentityRole<Guid>>> roleMock = GetMockRoleManager();
        private static Employee testEmployee = mapper.Map<Employee>(GetTestEmployees().FirstOrDefault(x => x.Id == _guids[0]));

        private static Mock<IRepository<Employee>> repositoryEmployeeMock = new Mock<IRepository<Employee>>();

        private static Mock<UserManager<Employee>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<Employee>>();
            return new Mock<UserManager<Employee>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        }
        public static Mock<RoleManager<IdentityRole<Guid>>> GetMockRoleManager()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole<Guid>>>();
            return new Mock<RoleManager<IdentityRole<Guid>>>(roleStore.Object, null, null, null, null);

        }

        private static List<EmployeeDTO> GetTestEmployees()
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

        private EmployeeService Init()
        {
            var testEmployee = mapper.Map<Employee>(GetTestEmployees().FirstOrDefault(x => x.Id == _guids[0]));
            repositoryEmployeeMock.Setup(x => x.Get(It.IsAny<Expression<Func<Employee, bool>>>())).Returns(testEmployee);
            repositoryEmployeeMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Employee, bool>>>()).Result).Returns(testEmployee);
            repositoryEmployeeMock.Setup(x => x.GetAll()).Returns(mapper.Map<ICollection<Employee>>(GetTestEmployees()).AsQueryable());
            repositoryEmployeeMock.Setup(x => x.Delete(It.IsAny<Employee>()));
            repositoryEmployeeMock.Setup(x => x.DeleteAsync(It.IsAny<Employee>()));
            userMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()).Result).Returns(testEmployee);

            uowMock.Setup(x => x.SaveChangesAsync());
            uowMock.Setup(x => x.Save());
            uowMock.Setup(x => x.GetRepository<Employee>()).Returns(repositoryEmployeeMock.Object);

            EmployeeService service = new EmployeeService(uowMock.Object, mapper, userMock.Object, roleMock.Object);
            return service;
        }
        #endregion

        [Fact]
        public void EmployeeAddItemIsNullException()
        {
            // Arrange
            EmployeeDTO item = null;
            var service = Init();
            // Assert
            Assert.Throws<ArgumentNullException>(() => service.Add(item));
        }

        [Fact]
        public void EmployeeWasAddedAndSaved()
        {
            // Arrange
            var employeeDTO = GetTestEmployees().FirstOrDefault(x => x.Id == _guids[0]);
            var service = Init();
            // Act
            service.Add(employeeDTO);
            // Assert
            repositoryEmployeeMock.Verify(u => u.Add(It.IsAny<Employee>()));
            uowMock.Verify(u => u.Save());
        }

        [Fact]
        public void EmployeeAddAsyncItemIsNullException()
        {
            // Arrange
            EmployeeDTO item = null;
            var service = Init();
            // Assert
            Assert.Throws<AggregateException>(() => service.AddAsync(item).Result);
        }

        [Fact]
        public async void EmployeeWasAddedAndSavedAsAsync()
        {
            // Arrange
            var employeeDTO = GetTestEmployees().FirstOrDefault(x => x.Id == _guids[0]);
            var service = Init();
            // Act
            var result = await service.AddAsync(employeeDTO);
            // Assert
            Assert.Equal(employeeDTO.Id, result.Id);
            repositoryEmployeeMock.Verify(u => u.AddAsync(It.IsAny<Employee>()));
            uowMock.Verify(u => u.SaveChangesAsync());
        }

        [Fact]
        public void DeleteEmployeeNotFoundException()
        {
            // Arrange
            repositoryEmployeeMock.Setup(x => x.Get(It.IsAny<Expression<Func<Employee, bool>>>())).Returns<Employee>(null);
            uowMock.Setup(x => x.GetRepository<Employee>()).Returns(repositoryEmployeeMock.Object);
            var service = new EmployeeService(uowMock.Object, mapper, userMock.Object, roleMock.Object);
            // Assert
            Assert.Throws<ArgumentException>(() => service.Delete(Guid.NewGuid()));
        }

        [Fact]
        public void EmployeeSuccsesfulyDeletedAndChangesSaved()
        {
            // Arrange
            repositoryEmployeeMock.Setup(x => x.Get(It.IsAny<Expression<Func<Employee, bool>>>())).Returns(testEmployee);
            uowMock.Setup(x => x.GetRepository<Employee>()).Returns(repositoryEmployeeMock.Object);
            var service = new EmployeeService(uowMock.Object, mapper, userMock.Object, roleMock.Object);
            // Act
            service.Delete(_guids[0]);
            // Assert
            repositoryEmployeeMock.Verify(u => u.Get(It.IsAny<Expression<Func<Employee, bool>>>()));
            repositoryEmployeeMock.Verify(u => u.Delete(testEmployee));
            uowMock.Verify(u => u.Save());
        }

        [Fact]
        public void DeleteEmployeeAsyncNotFoundException()
        {
            // Arrange
            var service = Init();
            // Assert
            Assert.ThrowsAsync<ArgumentException>(() => service.DeleteAsync(Guid.NewGuid()));
        }

        [Fact]
        public void DeleteAsyncAndChangesSaved()
        {
            // Arrange
            repositoryEmployeeMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Employee, bool>>>()).Result).Returns(testEmployee);
            uowMock.Setup(x => x.GetRepository<Employee>()).Returns(repositoryEmployeeMock.Object);
            var service = new EmployeeService(uowMock.Object, mapper, userMock.Object, roleMock.Object);
            // Act
            service.DeleteAsync(_guids[0]);
            // Assert
            repositoryEmployeeMock.Verify(u => u.GetAsync(It.IsAny<Expression<Func<Employee, bool>>>()));
            repositoryEmployeeMock.Verify(u => u.DeleteAsync(testEmployee));
            uowMock.Verify(u => u.SaveChangesAsync());
        }

        [Fact]
        public void ExecuteDispose()
        {
            // Arrange
            var service = Init();
            // Act
            service.Dispose();
            // Assert
            uowMock.Verify(u => u.Dispose());
        }

        [Fact]
        public void GetEmployeeReturnEmployeeDTO()
        {
            // Arrange
            var testEmployeeDTO = GetTestEmployees().FirstOrDefault(x => x.Id == _guids[0]);
            var service = Init();
            // Act
            var result = service.Get(x => x.Id == _guids[0]);
            // Assert
            Assert.Equal(testEmployeeDTO.Id, result.Id);
        }

        [Fact]
        public void GetAllEmployeesReturnCollectionEmployeeDTO()
        {
            // Arrange
            var service = Init();
            // Act
            var result = service.GetAll();
            // Assert
            Assert.Equal(GetTestEmployees().Count, result.Count);
        }

        [Fact]
        public void GetAsyncReturnProjectDTO()
        {
            // Arrange
            var testEmployeeDTO = GetTestEmployees().FirstOrDefault(x => x.Id == _guids[0]);
            var service = Init();
            // Act
            var result = service.GetAsync(x => x.Id == _guids[0]).Result;
            // Assert
            Assert.Equal(testEmployeeDTO.Id, result.Id);
        }

        [Fact]
        public async void UpdateAsyncEmployeeReturnNotFoundExeption()
        {
            // Arrange
            EmployeeDTO testEmployeeDTO = null;
            var service = Init();
            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateAsync(null));
        }

        [Fact]
        public async void UpdateAsyncEmployeeReturnsEmployeeDTO()
        {
            // Arrange
            var testEmployeeDTO = GetTestEmployees().FirstOrDefault(x => x.Id == _guids[0]);
            var service = Init();
            // Act
            var result = await service.UpdateAsync(testEmployeeDTO);
            // Assert
            Assert.Equal(testEmployeeDTO.Id, result.Id);
            userMock.Verify(u => u.GetRolesAsync(It.IsAny<Employee>()), Times.Never());
            userMock.Verify(u => u.RemoveFromRolesAsync(It.IsAny<Employee>(), It.IsAny<List<string>>()), Times.Never());
            userMock.Verify(u => u.AddToRolesAsync(It.IsAny<Employee>(), It.IsAny<List<string>>()), Times.Never());
            userMock.Verify(u => u.ChangeEmailAsync(It.IsAny<Employee>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            userMock.Verify(u => u.SetUserNameAsync(It.IsAny<Employee>(), It.IsAny<string>()), Times.Never());
            userMock.Verify(u => u.ResetPasswordAsync(It.IsAny<Employee>(), It.IsAny<string>(), It.IsAny<string>()));
            uowMock.Verify(u => u.SaveChangesAsync());
        }
    }
}
