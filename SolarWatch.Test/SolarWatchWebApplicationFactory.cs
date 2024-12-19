using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SolarWatch.Data;
using SolarWatch.Services.Authentication;

namespace SolarWatch.Test;

public class SolarWatchWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //Get the previous DbContextOptions registrations 
            var solarWatchDbContextDescriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<SolarWatchDbContext>));
            var usersDbContextDescriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UsersContext>));

            //Remove the previous DbContextOptions registrations
            if (solarWatchDbContextDescriptor != null)
            {
                services.Remove(solarWatchDbContextDescriptor);
            }

            if (usersDbContextDescriptor != null)
            {
                services.Remove(usersDbContextDescriptor);
            }

            //Add new DbContextOptions for our two contexts, this time with inmemory db 
            services.AddDbContext<SolarWatchDbContext>(options => { options.UseInMemoryDatabase(_dbName); });

            services.AddDbContext<UsersContext>(options => { options.UseInMemoryDatabase(_dbName); });

            //We will need to initialize our in memory databases. 
            //Since DbContexts are scoped services, we create a scope
            using var scope = services.BuildServiceProvider().CreateScope();
            var serviceProvider = scope.ServiceProvider;

            //We use this scope to request the registered dbcontexts, and initialize the schemas
            var solarContext = scope.ServiceProvider.GetRequiredService<SolarWatchDbContext>();
            solarContext.Database.EnsureDeleted();
            solarContext.Database.EnsureCreated();

            var userContext = scope.ServiceProvider.GetRequiredService<UsersContext>();
            userContext.Database.EnsureDeleted();
            userContext.Database.EnsureCreated();

            //Here we could do more initializing if we wished (e.g. adding admin user)
            // Seed data
            SeedData(serviceProvider).GetAwaiter().GetResult();
            builder.UseEnvironment("Development");
        });
        
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.AddDebug();
        });

    }

    private async Task SeedData(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
        var authenticationSeeder = serviceProvider.GetService<AuthenticationSeeder>();
        authenticationSeeder.AddRoles();  // This will add the roles based on your settings

        // Ensure any existing users are cleared out
        var existingUsers = userManager.Users.ToList();
        foreach (var existingUser in existingUsers)
        {
            await userManager.DeleteAsync(existingUser);
        }

        // Create and seed a new user
        var user = new IdentityUser
        {
            UserName = "testuser",
            Email = "testuser@example.com"
        };
        

        var password = "Test@1234"; // Ensure this meets the password policy
        var result = await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "User");
        
        if (!result.Succeeded)
        {
            throw new Exception("Failed to seed test user: " +
                                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}