using Microsoft.AspNetCore.Identity;

namespace ProjectManager.Domain.Entities
{
    public class Employee : IdentityUser<Guid>
    {
        public string? FName { get; set; }
        public string? SName { get; set; }
        public string? Patronymic { get; set; }
        public ICollection<Project>? Projects { get; set; }
        public ICollection<Objective>? Objectives { get; set; }
    }
}
