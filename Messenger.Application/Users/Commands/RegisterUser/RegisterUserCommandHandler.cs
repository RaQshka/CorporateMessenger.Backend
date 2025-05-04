using System.ComponentModel.DataAnnotations;
using MediatR;
using Messenger.Application.Interfaces;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Messenger.Application.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegistrationResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public RegisterUserCommandHandler(UserManager<User> userManager, IEmailSender emailSender, IConfiguration configuration)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _configuration = configuration;
    }

    public async Task<RegistrationResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userManager.FindByNameAsync(request.Username) != null)
            return new RegistrationResult
            {
                Message = "Пользователь с таким логином уже существует!"
            };

        if (await _userManager.FindByEmailAsync(request.Email) != null)
            return new RegistrationResult
            {
                Message = "Пользователь с такой почтой уже зарегистрирован!"
            };

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Username,
            Email = request.Email,
            CorporateKey = request.CorporateKey,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            RegistrationStatus = "PendingApproval",
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)            
            return new RegistrationResult
            {
                Message = "Возникли ошибки: " + string.Join(',', result.Errors.Select(e => e.Description))
            };
            
        
        await _userManager.AddToRoleAsync(user, "Employee");

        // Генерация токена подтверждения email
        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        // Формируем ссылку для письма и отправляем email
        var frontendUrl = _configuration["FrontendUrl"];
        var confirmationLink = $"{frontendUrl}/api/auth/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(confirmationToken)}";

        var emailMessage = $"Для подтверждения регистрации перейдите по ссылке: {confirmationLink}";
        await _emailSender.SendEmailAsync(request.Email, "Подтверждение Email", emailMessage);
        
        
        return new RegistrationResult
        {
            UserId = user.Id,
            EmailConfirmationToken = confirmationToken,
            Message = "Регистрация прошла успешно!"
        };
    }
}
