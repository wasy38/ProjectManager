using ProjectManager.Domain.Enums;

namespace ProjectManager.BLL.DTO
{
    public class ObjectiveDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Author { get; set; }
        public Guid ProjectId { get; set; }
        public ProjectDTO? Project { get; set; }
        public Status Status { get; set; }
        public string? Comment { get; set; }
        public int Priority { get; set; }
        public ICollection<EmployeeDTO>? Employees { get; set; }
    }
}
