using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManager.BLL.Interfaces;
using ProjectManager.BLL.Services;
using ProjectManager.DAL.Context;
using ProjectManager.DAL.Repositories;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces;
using ProjectManager.WEB;
using ProjectManager.WEB.AutoMapperProfiles;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ManagerDBContextConnection") ?? throw new InvalidOperationException("Connection string 'ManagerDBContextConnection' not found.");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ManagerDBContext>(options => options.UseSqlite("Data Source=DB.db"));

builder.Services.AddIdentity<Employee, IdentityRole<Guid>>(
    options =>
    {
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 5;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    }
)
.AddTokenProvider<DataProtectorTokenProvider<Employee>>(TokenOptions.DefaultProvider)
.AddDefaultUI()
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<ManagerDBContext>();

builder.Services.AddScoped<IUnitOfWork, EFUnitOfWork>();
builder.Services.AddAutoMapper(typeof(EmployeeProfile), typeof(ProjectProfile), typeof(ObjectiveProfile));
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IObjectiveService, ObjectiveService>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.MapRazorPages();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Project}/{action=Index}/{id?}");
SeedData.EnsureSeedData(app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);
app.Run();
