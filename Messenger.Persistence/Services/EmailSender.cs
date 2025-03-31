using System.Net;
using System.Net.Mail;
using MailKit.Security;
using Messenger.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Messenger.Persistence.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        // Настройки SMTP
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
        var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
        var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
        var senderEmail = _configuration["EmailSettings:SenderEmail"];

        // Создание сообщения
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(smtpUsername, senderEmail));
        mimeMessage.To.Add(new MailboxAddress(email, email)); // Замените на Email получателя
        mimeMessage.Subject = subject;
        mimeMessage.Body = new TextPart("plain")
        {
            Text = message
        };

        // Отправка сообщения
        try
        {
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                // Подключение к серверу SMTP
                await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.SslOnConnect);

                // Аутентификация
                await client.AuthenticateAsync(senderEmail, smtpPassword);

                // Отправка сообщения
                await client.SendAsync(mimeMessage);
                _logger.LogInformation($"Письмо успешно отправлено на {email}");

                // Отключение клиента
                await client.DisconnectAsync(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при отправке письма на {email}: {ex.Message}");

        }
    }

}