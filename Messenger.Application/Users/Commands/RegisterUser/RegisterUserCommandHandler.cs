using MediatR;
using Messenger.Application.Interfaces;

namespace Messenger.Application.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler:IRequestHandler<RegisterUserCommand, RegistrationResult> 
{
    private readonly IUserService _userService;
    private readonly IEmailSender _emailSender;
    private readonly IEmailConfirmationService _emailConfirmationService; // Может быть отдельный сервис для генерации токенов

    public RegisterUserCommandHandler(IUserService userService, IEmailSender emailSender, IEmailConfirmationService emailConfirmationService)
    {
        _userService = userService;
        _emailSender = emailSender;
        _emailConfirmationService = emailConfirmationService;
    }
    public async Task<RegistrationResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var userId = await _userService.RegisterAsync(
            request.Username, 
            request.Email, 
            request.Password, 
            request.CorporateKey, 
            request.FirstName, 
            request.LastName);   
        // Генерация токена для подтверждения email
        var token = await _emailConfirmationService.GenerateEmailConfirmationTokenAsync(userId);
        
  
        // Отправка email с подтверждением
        var confirmationLink = $"https://yourdomain.com/api/auth/confirmemail?userId={userId}&token={token}";
        var emailMessage = $"Для подтверждения регистрации перейдите по ссылке: {confirmationLink}";
        await _emailSender.SendEmailAsync(request.Email, "Подтверждение Email", emailMessage);
        
        // Возврат результата – регистрация прошла успешно, но заявка ожидает административной проверки
        return new RegistrationResult
        {
            UserId = userId,
            EmailConfirmationToken = token,
            Message = "Заявка на регистрацию успешно подана. Подтвердите email, перейдя по ссылке в письме."
        };
    }
}