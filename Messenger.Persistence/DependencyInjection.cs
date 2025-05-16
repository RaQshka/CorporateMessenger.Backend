using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Messenger.Persistence.Migrations;
using Messenger.Persistence.Repositories;
using Messenger.Persistence.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Messenger.Persistence;
public static class DependencyInjection
{
    public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<MessengerDbContext>(options => options.UseSqlServer(connectionString).EnableSensitiveDataLogging());

        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<MessengerDbContext>()
            .AddDefaultTokenProviders();

        // Добавляем UserManager, RoleManager и SignInManager в контейнер
        services.AddScoped<UserManager<User>>();
        services.AddScoped<RoleManager<Role>>();
        services.AddScoped<SignInManager<User>>();
        services.AddTransient<MessengerDbContextFactory>();
        // Аудит
        services.AddScoped<IAuditLogger, AuditLogger>();
        
        // JWT-сервис
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        
        // Email-сервис
        services.AddTransient<IEmailSender, EmailSender>();
        
        // Репозитории
        services.AddScoped<IMessengerDbContext>(provider => provider.GetRequiredService<MessengerDbContext>());
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IChatAccessRepository, ChatAccessRepository>();
        services.AddScoped<IChatParticipantRepository, ChatParticipantRepository>();

        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IDocumentAccessRepository, DocumentAccessRepository>();
        services.AddScoped<IReactionRepository, ReactionRepository>();
        
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IChatAccessService, ChatAccessService>();
        services.AddScoped<IChatParticipantService, ChatParticipantService>();
        
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IReactionService, ReactionService>();
        services.AddScoped<IDocumentService,DocumentService>();
        services.AddScoped<IDocumentAccessService, DocumentAccessService>();


        return services;
    }

    
}