namespace ProjectManager.WEB.ViewModels.EntityViewModel
{
    public class EmployeeViewModel
    {
        public Guid Id { get; set; }
        public string? FName { get; set; }
        public string? SName { get; set; }
        public string? Patronymic { get; set; }
        public string? Email { get; set; }
        public ICollection<ProjectViewModel>? Projects { get; set; }
        public Guid[] Roles { get; set; }
        public string? Password { get; set; }
    }
}
