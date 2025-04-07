using Messenger.Application.Interfaces;
using Messenger.Domain;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Persistence.Services;

public class UserAccessService : IUserAccessService
    {
        private readonly IChatRepository _chatRepository;
        private readonly UserManager<User> _userManager;

        public UserAccessService(IChatRepository chatRepository, UserManager<User> userManager)
        {
            _chatRepository = chatRepository;
            _userManager = userManager;
        }

        public async Task<bool> CanSendMessageAsync(Guid chatId, Guid userId)
        {
            // Получаем чат
            var chat = await _chatRepository.GetChatByIdAsync(chatId, default);
            if (chat == null)
                return false;
            
            // Если политика пуста, можно допустить отправку всеми
            if (string.IsNullOrWhiteSpace(chat.AccessPolicy))
                return true;

            // Предположим, AccessPolicy — это строка с допустимыми ролями, разделёнными запятыми.
            var allowedRoles = chat.AccessPolicy.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(r => r.Trim().ToLowerInvariant()).ToList();

            // Получаем роли пользователя
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;
            var userRoles = (await _userManager.GetRolesAsync(user)).Select(r => r.ToLowerInvariant()).ToList();

            // Если есть пересечение ролей, разрешаем отправку
            return allowedRoles.Any(role => userRoles.Contains(role));
        }

        public async Task<bool> CanJoinChatAsync(Guid chatId, Guid userId)
        {
            // Можно реализовать дополнительную логику.
            // Для простоты, разрешаем всем, если чат существует.
            var chat = await _chatRepository.GetChatByIdAsync(chatId, default);
            return chat != null;
        }

        public async Task<bool> CanAssignAdminAsync(Guid chatId, Guid userId)
        {
            // Обычно только создатель чата (CreatedBy) или текущие администраторы могут назначать администраторов.
            var chat = await _chatRepository.GetChatByIdAsync(chatId, default);
            if (chat == null)
                return false;

            // Если пользователь является создателем – разрешаем
            if (chat.CreatedBy == userId)
                return true;

            // Иначе можно проверить, есть ли у пользователя статус администратора в чате
            var participant = chat.ChatParticipants.FirstOrDefault(cp => cp.UserId == userId);
            return participant != null && participant.IsAdmin;
        }
    }
