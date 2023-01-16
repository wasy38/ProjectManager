namespace ProjectManager.BLL.DTO
{
    public class EmployeeDTO
    {
        public Guid Id { get; set; }
        public string? FName { get; set; }
        public string? SName { get; set; }
        public string? Patronymic { get; set; }
        public string? Email { get; set; }
        public ICollection<ProjectDTO>? Projects { get; set; }
        public Guid[] Roles { get; set; }
        public string? Password { get; set; }
    }
}
