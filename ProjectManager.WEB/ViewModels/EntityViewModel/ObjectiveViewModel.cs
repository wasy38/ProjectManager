using ProjectManager.Domain.Enums;

namespace ProjectManager.WEB.ViewModels.EntityViewModel
{
    public class ObjectiveViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Author { get; set; }
        public Guid ProjectId { get; set; }
        public ProjectViewModel? Project { get; set; }
        public Status Status { get; set; }
        public string? Comment { get; set; }
        public int Priority { get; set; }
        public ICollection<EmployeeViewModel>? Employees { get; set; }
    }
}
