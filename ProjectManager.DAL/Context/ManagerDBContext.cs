using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Domain.Entities;

namespace ProjectManager.DAL.Context
{
    public class ManagerDBContext : IdentityDbContext<Employee, IdentityRole<Guid>, Guid>
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Objective> Objectives { get; set; }

        public ManagerDBContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>().HasData(
                new Project
                {
                    Id = Guid.NewGuid(),
                    CustomerName = "ООО TestGrope",
                    Start = DateTime.Now,
                    Name = "Test",
                    Priority = 100,
                    PerformerName = "OAO TestPerformer"
                });
        }

    }
}
