﻿using Messenger.Application.Interfaces;
using Messenger.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Messenger.Persistence;
public static class DependencyInjection
{
    public static IServiceCollection AddPersistance(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DbConnection");
        services.AddDbContext<MessengerDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IMessengerDbContext>(provider => provider.GetRequiredService<MessengerDbContext>());
        services.AddTransient<IEmailSender, EmailSender>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        return services;
    }
}