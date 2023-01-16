using Microsoft.AspNetCore.Identity;
using ProjectManager.Domain.Entities;

namespace ProjectManager.WEB
{
    public class SeedData
    {
        public static async Task EnsureSeedData(IServiceProvider provider)
        {
            var _roleManager = provider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            foreach (var roleName in RoleNames.AllRoles)
            {
                var role = _roleManager.FindByNameAsync(roleName).Result;

                if (role == null)
                {
                    var result = _roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName }).Result;
                    if (!result.Succeeded) throw new Exception(result.Errors.First().Description);
                }
            }

            var _userManager = provider.GetRequiredService<UserManager<Employee>>();
            Employee defaultAdm = new Employee { Email = "Admin@admin", FName = "Admin", UserName = "Admin@admin", EmailConfirmed = true };
            var adm = await _userManager.CreateAsync(defaultAdm, "1qa2ws#ED");
            if (adm.Succeeded)
            {
                var admUser = await _userManager.FindByEmailAsync("Admin@admin");

                await _userManager.AddToRoleAsync(admUser, RoleNames.Admin);
            }
        }
    }

    public static class RoleNames
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string TeamLead = "TeamLead";

        public static IEnumerable<string> AllRoles
        {
            get
            {
                yield return Admin;
                yield return User;
                yield return TeamLead;
            }
        }
    }
}
