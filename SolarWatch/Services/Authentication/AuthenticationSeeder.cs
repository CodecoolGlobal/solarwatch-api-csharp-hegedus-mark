using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SolarWatch.Configuration;

namespace SolarWatch.Services.Authentication;

public class AuthenticationSeeder
{
    private RoleManager<IdentityRole> _roleManager;
    private UserManager<IdentityUser> _userManager;
    private readonly IOptions<RoleSettings> _options;

    public AuthenticationSeeder(RoleManager<IdentityRole> roleManager, IOptions<RoleSettings> options, UserManager<IdentityUser> userManager)
    {
        _roleManager = roleManager;
        _options = options;
        _userManager = userManager;
    }

    public void AddAdmin()
    {
        var tAdmin = CreateAdminIfNotExists();
        tAdmin.Wait();
    }

    public void AddRoles()
    {
        var tAdmin = CreateAdminRole(_roleManager);
        tAdmin.Wait();

        var tUser = CreateUserRole(_roleManager);
        tUser.Wait();
    }


    private async Task CreateAdminRole(RoleManager<IdentityRole> roleManager)
    {
        await roleManager.CreateAsync(new IdentityRole(_options.Value.AdminRoleName));
    }
    
    async Task CreateUserRole(RoleManager<IdentityRole> roleManager)
    {
        await roleManager.CreateAsync(new IdentityRole(_options.Value.UserRoleName)); 
    }
    
    private async Task CreateAdminIfNotExists()
    {
        var adminInDb = await _userManager.FindByEmailAsync("admin@admin.com");
        if (adminInDb == null)
        {
            var admin = new IdentityUser { UserName = "admin", Email = "admin@admin.com" };
            var adminCreated = await _userManager.CreateAsync(admin, "admin123");

            if (adminCreated.Succeeded)
            {
                await _userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}