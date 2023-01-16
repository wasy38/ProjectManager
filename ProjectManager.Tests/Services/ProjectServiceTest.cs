using AutoMapper;
using Moq;
using ProjectManager.BLL.DTO;
using ProjectManager.BLL.Services;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces;
using ProjectManager.WEB.AutoMapperProfiles;
using System.Linq.Expressions;

namespace ProjectManager.Tests.Services
{
    public class ProjectServiceTest
    {
        #region reference data
        private static Guid[] _guids = new Guid[5] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        private static MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> { new ProjectProfile(), new ObjectiveProfile(), new EmployeeProfile() }));
        private static Mapper mapper = new Mapper(config);
        private static Mock<IUnitOfWork> uowMock = new Mock<IUnitOfWork>();
        private static Mock<IRepository<Project>> repositoryProjectMock = new Mock<IRepository<Project>>();
        private static Mock<IRepository<Employee>> repositoryEmployeeMock = new Mock<IRepository<Employee>>();

        private static List<ProjectDTO> GetTestProjects()
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

        private ProjectService Init()
        {
            repositoryProjectMock.Setup(x => x.Get(It.IsAny<Expression<Func<Project, bool>>>())).Returns(mapper.Map<Project>(GetTestProjects().FirstOrDefault(x => x.Id == _guids[0])));
            repositoryProjectMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Project, bool>>>()).Result).Returns(mapper.Map<Project>(GetTestProjects().FirstOrDefault(x => x.Id == _guids[0])));
            repositoryProjectMock.Setup(x => x.GetAll()).Returns(mapper.Map<ICollection<Project>>(GetTestProjects()).AsQueryable());

            uowMock.Setup(x => x.GetRepository<Project>()).Returns(repositoryProjectMock.Object);
            uowMock.Setup(x => x.GetRepository<Employee>()).Returns(repositoryEmployeeMock.Object);

            var service = new ProjectService(uowMock.Object, mapper);
            return service;
        }
        #endregion

        [Fact]
        public void ProjectAddItemIsNullException()
        {
            // Arrange
            ProjectDTO item = null;
            var service = Init();
            // Assert
            Assert.Throws<ArgumentNullException>(() => service.Add(item));
        }

        [Fact]
        public void ProjectWasAddedAndSaved()
        {
            // Arrange
            var projectDTO = GetTestProjects().FirstOrDefault(x => x.Id == _guids[0]);
            var service = Init();
            // Act
            service.Add(projectDTO);
            // Assert
            repositoryProjectMock.Verify(u => u.Add(It.IsAny<Project>()));
            uowMock.Verify(u => u.Save());
        }

        [Fact]
        public void ProjectAddAsyncItemIsNullException()
        {
            // Arrange
            ProjectDTO item = null;
            var service = Init();
            // Assert
            Assert.Throws<AggregateException>(() => service.AddAsync(item).Result);
        }

        [Fact]
        public async void ProjectWasAddedAndSavedAsAsync()
        {
            // Arrange
            var projectDTO = GetTestProjects().FirstOrDefault(x => x.Id == _guids[0]);
            var service = Init();
            // Act
            var result = await service.AddAsync(projectDTO);
            // Assert
            Assert.Equal(projectDTO.Id, result.Id);
            repositoryProjectMock.Verify(u => u.AddAsync(It.IsAny<Project>()));
            repositoryEmployeeMock.Verify(u => u.GetAsync(It.IsAny<Expression<Func<Employee, bool>>>()), Times.Never());
            uowMock.Verify(u => u.SaveChangesAsync());
        }

        [Fact]
        public void DeleteProjectNotFoundException()
        {
            // Arrange
            repositoryProjectMock.Setup(x => x.Get(It.IsAny<Expression<Func<Project, bool>>>())).Returns<Project>(null);
            uowMock.Setup(x => x.GetRepository<Project>()).Returns(repositoryProjectMock.Object);
            var service = new ProjectService(uowMock.Object, mapper);
            // Assert
            Assert.Throws<ArgumentException>(() => service.Delete(Guid.NewGuid()));
        }

        [Fact]
        public void ProjectSuccsesfulyDeletedAndChangesSaved()
        {
            // Arrange
            var testProj = mapper.Map<Project>(GetTestProjects().FirstOrDefault(x => x.Id == _guids[0]));
            repositoryProjectMock.Setup(x => x.Get(It.IsAny<Expression<Func<Project, bool>>>())).Returns(testProj);
            uowMock.Setup(x => x.GetRepository<Project>()).Returns(repositoryProjectMock.Object);
            var service = new ProjectService(uowMock.Object, mapper);
            // Act
            service.Delete(_guids[0]);
            // Assert
            repositoryProjectMock.Verify(u => u.Get(It.IsAny<Expression<Func<Project, bool>>>()));
            repositoryProjectMock.Verify(u => u.Delete(testProj));
            uowMock.Verify(u => u.Save());
        }

        [Fact]
        public void DeleteProjectAsyncNotFoundException()
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
            var testProj = mapper.Map<Project>(GetTestProjects().FirstOrDefault(x => x.Id == _guids[0]));
            repositoryProjectMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Project, bool>>>()).Result).Returns(testProj);
            uowMock.Setup(x => x.GetRepository<Project>()).Returns(repositoryProjectMock.Object);

            var service = new ProjectService(uowMock.Object, mapper);
            // Act
            service.DeleteAsync(_guids[0]);
            // Assert
            repositoryProjectMock.Verify(u => u.GetAsync(It.IsAny<Expression<Func<Project, bool>>>()));
            repositoryProjectMock.Verify(u => u.DeleteAsync(testProj));
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
        public void GetProjectReturnProjectDTO()
        {
            // Arrange
            var testProjDTO = GetTestProjects().FirstOrDefault(x => x.Id == _guids[0]);
            var testProj = mapper.Map<Project>(testProjDTO);
            var service = Init();
            // Act
            var result = service.Get(x => x.Id == _guids[0]);
            // Assert
            Assert.Equal(testProjDTO.Id, result.Id);
        }

        [Fact]
        public void GetAllProjectReturnCollectionProjectDTO()
        {
            // Arrange
            var Projects = mapper.Map<ICollection<Project>>(GetTestProjects()).AsQueryable();
            var service = Init();
            // Act
            var result = service.GetAll();
            // Assert
            Assert.Equal(GetTestProjects().Count, result.Count);
        }

        [Fact]
        public void GetAsyncReturnProjectDTO()
        {
            // Arrange
            var testProjDTO = GetTestProjects().FirstOrDefault(x => x.Id == _guids[0]);
            var testProj = mapper.Map<Project>(testProjDTO);
            repositoryProjectMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), x => x.Employees, x => x.Objectives).Result).Returns(testProj);
            uowMock.Setup(x => x.GetRepository<Project>()).Returns(repositoryProjectMock.Object);
            var service = new ProjectService(uowMock.Object, mapper);
            // Act
            var result = service.GetAsync(x => x.Id == _guids[0]).Result;
            // Assert
            Assert.Equal(testProjDTO.Id, result.Id);
        }

        [Fact]
        public void UpdateProjectReturnNotFoundExeption()
        {
            // Arrange
            var testProjDTO = GetTestProjects().FirstOrDefault(x => x.Id == _guids[0]);
            var testProj = mapper.Map<Project>(testProjDTO);
            var service = Init();
            // Act
            var result = service.Update(testProjDTO);
            // Assert
            Assert.Throws<ArgumentNullException>(() => service.Update(null));
        }

        [Fact]
        public void UpdateProjectReturnsProjectDTO()
        {
            // Arrange
            var testProjDTO = GetTestProjects().FirstOrDefault(x => x.Id == _guids[0]);
            var testProj = mapper.Map<Project>(testProjDTO);
            repositoryProjectMock.Setup(x => x.Get(It.IsAny<Expression<Func<Project, bool>>>())).Returns(testProj);
            repositoryProjectMock.Setup(x => x.Update(testProj)).Returns(testProj);
            uowMock.Setup(x => x.GetRepository<Project>()).Returns(repositoryProjectMock.Object);
            var service = new ProjectService(uowMock.Object, mapper);            // Act
            var result = service.Update(testProjDTO);
            // Assert
            Assert.Equal(testProjDTO.Id, result.Id);
            uowMock.Verify(u => u.Save());
        }

        [Fact]
        public void UpdateAsyncProjectReturnNotFoundExeption()
        {
            // Arrange
            var testProjDTO = GetTestProjects().FirstOrDefault(x => x.Id == _guids[0]);
            var testProj = mapper.Map<Project>(testProjDTO);
            var service = Init();
            // Act
            var result = service.UpdateAsync(testProjDTO);
            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateAsync(null));
        }

        [Fact]
        public void UpdateAsyncProjectReturnsProjectDTO()
        {
            // Arrange
            var testProjDTO = GetTestProjects().FirstOrDefault(x => x.Id == _guids[0]);
            var testProj = mapper.Map<Project>(testProjDTO);
            repositoryProjectMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), x => x.Employees).Result).Returns(testProj);
            repositoryEmployeeMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Employee, bool>>>()).Result).Returns<Employee>(null);
            repositoryProjectMock.Setup(x => x.UpdateAsync(testProj).Result).Returns(testProj);
            uowMock.Setup(x => x.GetRepository<Project>()).Returns(repositoryProjectMock.Object);
            uowMock.Setup(x => x.GetRepository<Employee>()).Returns(repositoryEmployeeMock.Object);
            var service = new ProjectService(uowMock.Object, mapper);
            // Act
            var result = service.UpdateAsync(testProjDTO).Result;
            // Assert
            Assert.Equal(testProjDTO.Id, result.Id);
            uowMock.Verify(u => u.SaveChangesAsync());
        }
    }
}
