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
    public class ObjectiveServiceTest
    {
        #region reference data

        private static Guid[] _guids = new Guid[5] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        private static MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> { new ProjectProfile(), new ObjectiveProfile(), new EmployeeProfile() }));
        private static Mapper mapper = new Mapper(config);
        private static Mock<IUnitOfWork> uowMock = new Mock<IUnitOfWork>();
        private static Mock<IRepository<Objective>> repositoryObjectiveMock = new Mock<IRepository<Objective>>();
        private static Mock<IRepository<Project>> repositoryProjectMock = new Mock<IRepository<Project>>();
        private static Mock<IRepository<Employee>> repositoryEmployeeMock = new Mock<IRepository<Employee>>();

        private static Objective testObjective = mapper.Map<Objective>(GetTestObjectives().FirstOrDefault(x => x.Id == _guids[0]));
        private static ProjectDTO testProject = new ProjectDTO()
        {
            Id = _guids[4],
            Name = "PROJECT",
            Employees = new List<EmployeeDTO>()
            {
                new EmployeeDTO { Id = _guids[3] }
            }
        };

        private static List<ObjectiveDTO> GetTestObjectives()
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

        private ObjectiveService Init()
        {
            repositoryObjectiveMock.Setup(x => x.Get(It.IsAny<Expression<Func<Objective, bool>>>())).Returns(testObjective);
            repositoryObjectiveMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Objective, bool>>>(), x => x.Project, x => x.Employees).Result).Returns(testObjective);
            repositoryObjectiveMock.Setup(x => x.GetAll()).Returns(mapper.Map<ICollection<Objective>>(GetTestObjectives()).AsQueryable());
            repositoryObjectiveMock.Setup(x => x.Update(testObjective)).Returns(testObjective);
            repositoryEmployeeMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Employee, bool>>>()).Result).Returns<Employee>(null);
            repositoryObjectiveMock.Setup(x => x.UpdateAsync(It.IsAny<Objective>()).Result).Returns(testObjective);


            uowMock.Setup(x => x.GetRepository<Employee>()).Returns(repositoryEmployeeMock.Object);
            uowMock.Setup(x => x.GetRepository<Objective>()).Returns(repositoryObjectiveMock.Object);


            var service = new ObjectiveService(uowMock.Object, mapper);
            return service;
        }

        #endregion

        [Fact]
        public void ObjectiveAddItemIsNullException()
        {
            // Arrange
            ObjectiveDTO item = null;
            var service = Init();
            // Assert
            Assert.Throws<ArgumentNullException>(() => service.Add(item));
        }

        [Fact]
        public void ObjectiveWasAddedAndSaved()
        {
            // Arrange
            var objectiveDTO = GetTestObjectives().FirstOrDefault(x => x.Id == _guids[0]);
            var service = Init();
            // Act
            service.Add(objectiveDTO);
            // Assert
            repositoryObjectiveMock.Verify(u => u.Add(It.IsAny<Objective>()));
            uowMock.Verify(u => u.Save());
        }

        [Fact]
        public void ObjectiveAddAsyncItemIsNullException()
        {
            // Arrange
            ObjectiveDTO item = null;
            var service = Init();
            // Assert
            Assert.Throws<AggregateException>(() => service.AddAsync(item).Result);

        }

        [Fact]
        public async void ObjectiveWasAddedAndSavedAsAsync()
        {
            // Arrange
            var objectiveDTO = GetTestObjectives().FirstOrDefault(x => x.Id == _guids[0]);
            objectiveDTO.ProjectId = testProject.Id;
            var objective = mapper.Map<Objective>(objectiveDTO);
            repositoryObjectiveMock.Setup(x => x.AddAsync(It.IsAny<Objective>()).Result).Returns(objective);
            repositoryProjectMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Project, bool>>>()).Result).Returns(mapper.Map<Project>(testProject));
            uowMock.Setup(x => x.GetRepository<Objective>()).Returns(repositoryObjectiveMock.Object);
            uowMock.Setup(x => x.GetRepository<Project>()).Returns(repositoryProjectMock.Object);
            uowMock.Setup(x => x.GetRepository<Employee>()).Returns(repositoryEmployeeMock.Object);
            var service = new ObjectiveService(uowMock.Object, mapper);
            // Act
            var result = await service.AddAsync(objectiveDTO);
            // Assert
            Assert.Equal(objectiveDTO.Id, result.Id);
            Assert.Equal(testProject.Id, result.ProjectId);
            repositoryProjectMock.Verify(u => u.GetAsync(It.IsAny<Expression<Func<Project, bool>>>()));
            uowMock.Verify(u => u.SaveChangesAsync());
        }

        [Fact]
        public void DeleteProjectNotFoundException()
        {
            // Arrange
            repositoryObjectiveMock.Setup(x => x.Get(It.IsAny<Expression<Func<Objective, bool>>>())).Returns<Objective>(null);
            uowMock.Setup(x => x.GetRepository<Objective>()).Returns(repositoryObjectiveMock.Object);
            var service = new ObjectiveService(uowMock.Object, mapper);
            // Assert
            Assert.Throws<ArgumentException>(() => service.Delete(Guid.NewGuid()));
        }

        [Fact]
        public void ProjectSuccsesfulyDeletedAndChangesSaved()
        {
            // Arrange
            var service = Init();
            // Act
            service.Delete(testObjective.Id);
            // Assert
            repositoryObjectiveMock.Verify(u => u.Get(It.IsAny<Expression<Func<Objective, bool>>>()));
            repositoryObjectiveMock.Verify(u => u.Delete(testObjective));
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
            repositoryObjectiveMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Objective, bool>>>()).Result).Returns(testObjective);
            uowMock.Setup(x => x.GetRepository<Objective>()).Returns(repositoryObjectiveMock.Object);
            var service = new ObjectiveService(uowMock.Object, mapper);
            // Act
            service.DeleteAsync(_guids[0]);
            // Assert
            repositoryObjectiveMock.Verify(u => u.GetAsync(It.IsAny<Expression<Func<Objective, bool>>>()));
            repositoryObjectiveMock.Verify(u => u.DeleteAsync(testObjective));
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
        public void GetObjectiveReturnObjectiveDTO()
        {
            // Arrange
            var testObjectiveDTO = GetTestObjectives().FirstOrDefault(x => x.Id == _guids[0]);
            var service = Init();
            // Act
            var result = service.Get(x => x.Id == _guids[0]);
            // Assert
            Assert.Equal(testObjectiveDTO.Id, result.Id);
        }

        [Fact]
        public void GetAllObjectivesReturnCollectionObjectivesDTO()
        {
            // Arrange
            var service = Init();
            // Act
            var result = service.GetAll();
            // Assert
            Assert.Equal(GetTestObjectives().Count, result.Count);
        }

        [Fact]
        public async void GetAsyncReturnObjectiveDTO()
        {
            // Arrange
            var testObjectiveDTO = GetTestObjectives().FirstOrDefault(x => x.Id == _guids[0]);
            var service = Init();
            // Act
            var result = await service.GetAsync(x => x.Id == _guids[0]);
            // Assert
            Assert.Equal(testObjectiveDTO.Id, result.Id);
        }

        [Fact]
        public void UpdateObjectiveReturnNotFoundExeption()
        {
            // Arrange
            var testObjectiveDTO = GetTestObjectives().FirstOrDefault(x => x.Id == _guids[0]);
            var service = Init();
            // Act
            var result = service.Update(testObjectiveDTO);
            // Assert
            Assert.Throws<ArgumentNullException>(() => service.Update(null));
        }

        [Fact]
        public void UpdateObjectiveReturnsObjectiveDTO()
        {
            // Arrange
            var testObjectiveDTO = GetTestObjectives().FirstOrDefault(x => x.Id == _guids[0]);
            var service = Init();
            // Act
            var result = service.Update(testObjectiveDTO);
            // Assert
            Assert.Equal(testObjectiveDTO.Id, result.Id);
            uowMock.Verify(u => u.Save());
        }

        [Fact]
        public void UpdateAsyncObjectiveReturnNotFoundExeption()
        {
            // Arrange
            var testObjectiveDTO = GetTestObjectives().FirstOrDefault(x => x.Id == _guids[0]);
            var service = Init();
            // Act
            var result = service.UpdateAsync(testObjectiveDTO);
            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateAsync(null));
        }

        [Fact]
        public async void UpdateAsyncProjectReturnsProjectDTO()
        {
            // Arrange
            var testObjectiveDTO = GetTestObjectives().FirstOrDefault(x => x.Id == _guids[0]);
            var service = new ObjectiveService(uowMock.Object, mapper);
            // Act
            var result = await service.UpdateAsync(testObjectiveDTO);
            // Assert
            Assert.Equal(testObjectiveDTO.Id, result.Id);
            uowMock.Verify(u => u.SaveChangesAsync());
        }
    }
}
