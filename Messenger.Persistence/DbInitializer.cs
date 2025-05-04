using Messenger.Domain;
using Messenger.Domain.Entities;
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
        var myArraySection = configuration.GetSection("DefaultRoles").GetChildren();
        var roleNames = myArraySection.ToList();
        
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName.Value))
            {
                await roleManager.CreateAsync(new Role { Name = roleName.Value });
            }
        }
        context.Database.EnsureCreated();
    }
}