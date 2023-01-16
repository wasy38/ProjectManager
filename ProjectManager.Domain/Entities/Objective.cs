using ProjectManager.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Domain.Entities
{
    public class Objective
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Author { get; set; }
        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }
        public Status Status { get; set; }
        public string? Comment { get; set; }
        public int Priority { get; set; }
        public ICollection<Employee>? Employees { get; set; }
    }
}
