using Messenger.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Messenger.Persistence;

public class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        var context = services.GetRequiredService<MessengerDbContext>();
        var configuration = services.GetRequiredService<IConfiguration>();
        IConfigurationSection myArraySection = configuration.GetSection("DefaultRoles");
        var roleNames = myArraySection.AsEnumerable();
        
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName.Key))
            {
                await roleManager.CreateAsync(new Role { Name = roleName.Key });
            }
        }
        context.Database.EnsureCreated();
    }
}