﻿using System.Reflection;
using FluentValidation;
using MediatR;
using Messenger.Application.Chats.Commands.CreateChat;
using Messenger.Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(typeof(CreateChatCommandValidator).Assembly);
        services.AddTransient(
            typeof(IPipelineBehavior<,>), 
            typeof(ValidationBehavior<,>));
        return services;
    }
}