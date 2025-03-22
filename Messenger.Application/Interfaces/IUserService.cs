using Messenger.Domain;

namespace Messenger.Application.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Аутентификация пользователя по логину и паролю.
    /// </summary>
    Task<User> AuthenticateAsync(string username, string password);

    /// <summary>
    /// Регистрация нового пользователя с корпоративным ключом.
    /// </summary>
    Task<Guid> RegisterAsync(string username, string email, string password, string corporateKey, string firstName, string lastName);

    /// <summary>
    /// Проверка, существует ли пользователь с заданным логином.
    /// </summary>
    Task<bool> UserExistsAsync(string username);

    /// <summary>
    /// Подтверждение email пользователя по токену.
    /// </summary>
    Task<bool> ConfirmEmailAsync(Guid userId, string token);

    /// <summary>
    /// Обработка административного решения по заявке на регистрацию.
    /// </summary>

}
