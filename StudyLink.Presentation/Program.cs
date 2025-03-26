using StudyLink.Application;
using StudyLink.Application.Interfaces;
using StudyLink.Infrastructure.Repositories;
using StudyLink.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using StudyLink.Application.Services.Interface;
using StudyLink.Application.Services.Implementation;
using Microsoft.AspNetCore.Identity;
using StudyLink.Domain.Entities;
using StudyLink.Application.Mapper;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StudyLinkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StudyLinkContextConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<StudyLinkDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHangfire(config =>
           config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                 .UseSimpleAssemblyNameTypeSerializer()
                 .UseRecommendedSerializerSettings()
                 .UseSqlServerStorage(builder.Configuration.GetConnectionString("StudyLinkContextConnection")));

builder.Services.AddHangfireServer();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var rolesManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Teacher", "Student" };
    foreach (var role in roles)
    {
        if (!await rolesManager.RoleExistsAsync(role))
        {
            await rolesManager.CreateAsync(new IdentityRole(role));
        }
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    string email = "admin@gmail.com";
    string password = "Admin@1234";
    string firstName = "Admin";
    string lastName = "Admin";
    string address = "Kathmandu";
    var result = await userManager.FindByEmailAsync(email);
    if (result== null)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Address = address
        };

        var createResult = await userManager.CreateAsync(user, password);

        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}

app.Run();
