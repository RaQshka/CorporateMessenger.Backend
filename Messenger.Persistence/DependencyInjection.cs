using Messenger.Application.Interfaces;
using Messenger.Domain;
using Messenger.Persistence.Migrations;
using Messenger.Persistence.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Messenger.Persistence;
public static class DependencyInjection
{
    /*public static IServiceCollection AddPersistance(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DbConnection");
        services.AddDbContext<MessengerDbContext>(options => options.UseSqlServer(connectionString));
        
        services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<MessengerDbContext>()
            .AddDefaultTokenProviders();
        
        services.AddScoped<IMessengerDbContext>(provider => provider.GetRequiredService<MessengerDbContext>());
        services.AddTransient<IEmailSender, EmailSender>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        return services;
    }*/
    public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<MessengerDbContext>(options => options.UseSqlServer(connectionString));

        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<MessengerDbContext>()
            .AddDefaultTokenProviders();

        // Добавляем UserManager, RoleManager и SignInManager в контейнер
        services.AddScoped<UserManager<User>>();
        services.AddScoped<RoleManager<Role>>();
        services.AddScoped<SignInManager<User>>();
        services.AddTransient<MessengerDbContextFactory>();
        // JWT-сервис
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        
        // Аудит
        services.AddScoped<IAuditLogger, AuditLogger>();
        
        // Email-сервис
        services.AddTransient<IEmailSender, EmailSender>();
        
        // Репозитории
        services.AddScoped<IMessengerDbContext>(provider => provider.GetRequiredService<MessengerDbContext>());
        services.AddScoped<IMessageRepository, MessageRepository>();
        return services;
    }

    
}