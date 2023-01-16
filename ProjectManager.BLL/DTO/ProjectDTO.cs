namespace ProjectManager.BLL.DTO
{
    public class ProjectDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? CustomerName { get; set; }
        public string? PerformerName { get; set; }
        public Guid SupervisorId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Priority { get; set; }
        public ICollection<EmployeeDTO>? Employees { get; set; }
        public ICollection<ObjectiveDTO>? Objectives { get; set; }
    }
}
