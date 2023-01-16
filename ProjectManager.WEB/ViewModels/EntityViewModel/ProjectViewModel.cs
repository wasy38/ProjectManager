namespace ProjectManager.WEB.ViewModels.EntityViewModel
{
    public class ProjectViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? CustomerName { get; set; }
        public string? PerformerName { get; set; }
        public Guid SupervisorId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Priority { get; set; }
        public ICollection<EmployeeViewModel>? Employees { get; set; }
        public ICollection<ObjectiveViewModel>? Objectives { get; set; }
    }
}
