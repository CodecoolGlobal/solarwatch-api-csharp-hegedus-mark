using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SolarWatch.Configuration;

namespace SolarWatch.Services.Authentication;

public class AuthenticationSeeder
{
    private RoleManager<IdentityRole> _roleManager;
    private readonly IOptions<RoleSettings> _options;

    public AuthenticationSeeder(RoleManager<IdentityRole> roleManager, IOptions<RoleSettings> options)
    {
        _roleManager = roleManager;
        _options = options;
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
}