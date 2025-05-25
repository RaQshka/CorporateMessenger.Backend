using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Messenger.Application.Chats.Commands.AddUserToChat;
using Messenger.Application.Chats.Commands.CreateChat;
using Messenger.Application.Chats.Commands.DeleteChat;
using Messenger.Application.Chats.Commands.GrantChatAccess;
using Messenger.Application.Chats.Commands.RemoveUserFromChat;
using Messenger.Application.Chats.Commands.RenameChat;
using Messenger.Application.Chats.Commands.RevokeChatAccess;
using Messenger.Application.Chats.Commands.SetChatAdmin;
using Messenger.Application.Chats.Queries.GetChatAccessRules;
using Messenger.Application.Chats.Queries.GetChatActivity;
using Messenger.Application.Chats.Queries.GetChatInfo;
using Messenger.Application.Chats.Queries.GetChatParticipants;
using Messenger.Application.Chats.Queries.GetUserChats;
using Messenger.Application.Chats.Queries.Shared;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Messenger.WebApi.Controllers;
using Messenger.WebApi.Models.ChatDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Messenger.Tests.Controllers
{
    public class ChatControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IAuditLogger> _auditLoggerMock;
        private readonly ChatController _controller;
        private readonly Guid _testUserId = Guid.NewGuid();

        public ChatControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _auditLoggerMock = new Mock<IAuditLogger>();
            _controller = new ChatController(_mediatorMock.Object, _auditLoggerMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString())
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        private Dictionary<string, string> DeserializeResponse(OkObjectResult okResult)
        {
            // Способ 1: Через сериализацию (надежный способ)
            var json = JsonSerializer.Serialize(okResult.Value);
            var response = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            return response ?? new Dictionary<string, string>();
        }        
        
        
        #region Успешные сценарии

        [Fact]
        public async Task GetUserChats_ShouldReturnOkWithChats()
        {
            // Arrange
            var expectedChats = new List<UserChatDto>
            {
                new UserChatDto { Id = Guid.NewGuid(), Name = "Test Chat", Type = ChatTypes.Group.ToString() }
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetUserChatsQuery>(q => q.UserId == _testUserId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedChats);

            // Act
            var result = await _controller.GetUserChats();
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(expectedChats, okResult.Value);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "GetUserChats", "Chat", _testUserId,
                "Список чатов пользователя успешно получен", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task CreateChat_ShouldReturnOkWithChatId()
        {
            // Arrange
            var dto = new CreateChatDto { Name = "New Chat", Type = ChatTypes.Group };
            var expectedChatId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.Is<CreateChatCommand>(c => c.Name == dto.Name &&
                                                                          c.Type == dto.Type &&
                                                                          c.CreatorId == _testUserId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedChatId);

            // Act
            var result = await _controller.CreateChat(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = DeserializeResponse(okResult);
            
            Assert.Equal(expectedChatId.ToString(), response["ChatId"]);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "CreateChat", "Chat", expectedChatId,
                "Чат успешно создан", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task DeleteChat_ShouldReturnOk()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.Is<DeleteChatCommand>(c => c.ChatId == chatId &&
                                                                          c.InitiatorId == _testUserId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.DeleteChat(chatId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = DeserializeResponse(okResult);
            Assert.Equal("Чат успешно удален", response["message"]);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "DeleteChat", "Chat", chatId,
                "Чат успешно удален", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task RenameChat_ShouldReturnOk()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var dto = new RenameChatDto { NewName = "Renamed Chat" };
            _mediatorMock.Setup(m => m.Send(It.Is<RenameChatCommand>(c => c.ChatId == chatId &&
                                                                          c.NewName == dto.NewName &&
                                                                          c.InitiatorId == _testUserId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.RenameChat(chatId, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = DeserializeResponse(okResult);
            Assert.Equal("Чат успешно переименован", response["message"]);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "RenameChat", "Chat", chatId,
                "Чат успешно переименован", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task AddUserToChat_ShouldReturnOk()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dto = new AddUserDto { UserId = userId };
            _mediatorMock.Setup(m => m.Send(It.Is<AddUserToChatCommand>(c => c.ChatId == chatId &&
                                                                             c.UserId == userId &&
                                                                             c.InitiatorId == _testUserId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.AddUserToChat(chatId, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = DeserializeResponse(okResult);
            Assert.Equal("Пользователь успешно добавлен в чат", response["message"]);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "AddUserToChat", "Chat", chatId,
                "Пользователь успешно добавлен в чат", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task RemoveUserFromChat_ShouldReturnNoContent()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.Is<RemoveUserFromChatCommand>(c => c.ChatId == chatId &&
                                                                                  c.UserId == userId &&
                                                                                  c.InitiatorId == _testUserId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.RemoveUserFromChat(chatId, userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "RemoveUserFromChat", "Chat", chatId,
                "Пользователь успешно удален из чата", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task SetChatAdmin_ShouldReturnOk()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dto = new SetAdminDto { IsAdmin = true };
            _mediatorMock.Setup(m => m.Send(It.Is<SetChatAdminCommand>(c => c.ChatId == chatId &&
                                                                            c.UserId == userId &&
                                                                            c.IsAdmin == dto.IsAdmin &&
                                                                            c.InitiatorId == _testUserId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.SetChatAdmin(chatId, userId, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = DeserializeResponse(okResult);
            Assert.Equal("Статус администратора успешно обновлен", response["message"]);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "SetChatAdmin", "Chat", chatId,
                "Статус администратора изменен на True", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task GrantChatAccess_ShouldReturnOk()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var dto = new AccessDto { RoleId = Guid.NewGuid(), Access = ChatAccess.ReadMessages };
            _mediatorMock.Setup(m => m.Send(It.Is<GrantChatAccessCommand>(c => c.ChatId == chatId &&
                                                                               c.RoleId == dto.RoleId &&
                                                                               c.Access == dto.Access &&
                                                                               c.InitiatorId == _testUserId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.GrantChatAccess(chatId, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = DeserializeResponse(okResult);
            Assert.Equal("Доступ успешно предоставлен", response["message"]);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "GrantChatAccess", "Chat", chatId,
                "Доступ успешно предоставлен", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task RevokeChatAccess_ShouldReturnOk()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var dto = new AccessDto { RoleId = Guid.NewGuid(), Access = ChatAccess.ReadMessages };
            _mediatorMock.Setup(m => m.Send(It.Is<RevokeChatAccessCommand>(c => c.ChatId == chatId &&
                                                                                c.RoleId == dto.RoleId &&
                                                                                c.Access == dto.Access &&
                                                                                c.InitiatorId == _testUserId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.RevokeChatAccess(chatId, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = DeserializeResponse(okResult);
            Assert.Equal("Доступ успешно отозван", response["message"]);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "RevokeChatAccess", "Chat", chatId,
                "Доступ успешно отозван", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task GetChatInfo_ShouldReturnOkWithChatInfo()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var expectedChatInfo = new ChatInfoDto { Id = chatId, Name = "Test Chat", Type = (int)ChatTypes.Group };
            _mediatorMock.Setup(m => m.Send(It.Is<GetChatInfoQuery>(q => q.ChatId == chatId &&
                                                                         q.InitiatorId == _testUserId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedChatInfo);

            // Act
            var result = await _controller.GetChatInfo(chatId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedChatInfo, okResult.Value);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "GetChatInfo", "Chat", chatId,
                "Информация о чате успешно получена", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task GetChatParticipants_ShouldReturnOkWithParticipants()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var expectedParticipants = new List<ChatParticipantDto>
            {
                new ChatParticipantDto { UserId = Guid.NewGuid(), Username = "TestUser", IsAdmin = false }
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetChatParticipantsQuery>(q => q.ChatId == chatId &&
                                                                                 q.InitiatorId == _testUserId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedParticipants);

            // Act
            var result = await _controller.GetChatParticipants(chatId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedParticipants, okResult.Value);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "GetChatParticipants", "Chat", chatId,
                "Список участников чата успешно получен", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task GetChatAccessRules_ShouldReturnOkWithRules()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var expectedRules = new List<ChatAccessRuleDto>
            {
                new ChatAccessRuleDto { RoleId = Guid.NewGuid(), RoleName = "User", AccessMask = (int)ChatAccess.ReadMessages }
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetChatAccessRulesQuery>(q => q.ChatId == chatId &&
                                                                                q.InitiatorId == _testUserId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRules);

            // Act
            var result = await _controller.GetChatAccessRules(chatId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedRules, okResult.Value);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "GetChatAccessRules", "Chat", chatId,
                "Правила доступа чата успешно получены", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task GetChatActivity_ShouldReturnOkWithActivity()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var expectedActivity = new List<ChatActivityDto>
            {
                new ChatActivityDto { Id = Guid.NewGuid(), Type = "Message", Timestamp = DateTime.UtcNow }
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetChatActivityQuery>(q => q.ChatId == chatId &&
                                                                             q.UserId == _testUserId &&
                                                                             q.Skip == 0 &&
                                                                             q.Take == 50),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedActivity);

            // Act
            var result = await _controller.GetChatActivity(chatId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedActivity, okResult.Value);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "GetChatActivity", "Chat", chatId,
                "Активность чата получена", LogLevel.Info), Times.Once());
        }

        #endregion

        #region Сценарии с исключениями

        [Fact]
        public async Task CreateChat_ShouldThrowValidationException_WhenNameIsEmpty()
        {
            // Arrange
            var dto = new CreateChatDto { Name = "", Type = ChatTypes.Group };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateChatCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("Имя чата", "Имя чата не может быть пустым"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                async () => await _controller.CreateChat(dto));
            Assert.Equal("Ошибка валидации для поля 'Имя чата': Имя чата не может быть пустым", exception.Message);
        }

        [Fact]
        public async Task DeleteChat_ShouldThrowAccessDeniedException_WhenAccessDenied()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteChatCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AccessDeniedException("Удаление чата", chatId, _testUserId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AccessDeniedException>(
                async () => await _controller.DeleteChat(chatId));
            Assert.Equal($"Отказано в доступе для пользователя {_testUserId} при выполнении действия 'Удаление чата' в чате {chatId}.", exception.Message);
        }

        [Fact]
        public async Task DeleteChat_ShouldThrowNotFoundException_WhenChatNotExists()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteChatCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Чат", chatId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.DeleteChat(chatId));
            Assert.Equal($"Сущность \"Чат\" с идентификатором {chatId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task RenameChat_ShouldThrowValidationException_WhenNameIsEmpty()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var dto = new RenameChatDto { NewName = "" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RenameChatCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("Новое имя чата", "Новое имя чата не может быть пустым"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                async () => await _controller.RenameChat(chatId, dto));
            Assert.Equal("Ошибка валидации для поля 'Новое имя чата': Новое имя чата не может быть пустым", exception.Message);
        }

        [Fact]
        public async Task RenameChat_ShouldThrowAccessDeniedException_WhenAccessDenied()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var dto = new RenameChatDto { NewName = "Renamed Chat" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RenameChatCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AccessDeniedException("Изменение названия чата", chatId, _testUserId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AccessDeniedException>(
                async () => await _controller.RenameChat(chatId, dto));
            Assert.Equal($"Отказано в доступе для пользователя {_testUserId} при выполнении действия 'Изменение названия чата' в чате {chatId}.", exception.Message);
        }

        [Fact]
        public async Task AddUserToChat_ShouldThrowBusinessRuleException_WhenUserAlreadyInChat()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dto = new AddUserDto { UserId = userId };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddUserToChatCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BusinessRuleException("Пользователь уже является участником чата"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                async () => await _controller.AddUserToChat(chatId, dto));
            Assert.Equal("Нарушение бизнес-правила: Пользователь уже является участником чата", exception.Message);
        }

        [Fact]
        public async Task AddUserToChat_ShouldThrowBusinessRuleException_WhenDialogHasTwoParticipants()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dto = new AddUserDto { UserId = userId };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddUserToChatCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BusinessRuleException("В диалог нельзя добавить суммарно больше двух человек"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                async () => await _controller.AddUserToChat(chatId, dto));
            Assert.Equal("Нарушение бизнес-правила: В диалог нельзя добавить суммарно больше двух человек", exception.Message);
        }

        [Fact]
        public async Task AddUserToChat_ShouldThrowNotFoundException_WhenUserNotExists()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dto = new AddUserDto { UserId = userId };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddUserToChatCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Пользователь", userId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.AddUserToChat(chatId, dto));
            Assert.Equal($"Сущность \"Пользователь\" с идентификатором {userId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task RemoveUserFromChat_ShouldThrowBusinessRuleException_WhenRemovingCreator()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveUserFromChatCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BusinessRuleException("Нельзя удалить создателя чата"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                async () => await _controller.RemoveUserFromChat(chatId, userId));
            Assert.Equal("Нарушение бизнес-правила: Нельзя удалить создателя чата", exception.Message);
        }

        [Fact]
        public async Task RemoveUserFromChat_ShouldThrowNotFoundException_WhenUserNotExists()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveUserFromChatCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Пользователь", userId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.RemoveUserFromChat(chatId, userId));
            Assert.Equal($"Сущность \"Пользователь\" с идентификатором {userId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task SetChatAdmin_ShouldThrowBusinessRuleException_WhenRevokingCreatorAdmin()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dto = new SetAdminDto { IsAdmin = false };
            _mediatorMock.Setup(m => m.Send(It.IsAny<SetChatAdminCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BusinessRuleException("Создатель чата всегда должен быть администратором"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                async () => await _controller.SetChatAdmin(chatId, userId, dto));
            Assert.Equal("Нарушение бизнес-правила: Создатель чата всегда должен быть администратором", exception.Message);
        }

        [Fact]
        public async Task SetChatAdmin_ShouldThrowAccessDeniedException_WhenAccessDenied()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dto = new SetAdminDto { IsAdmin = true };
            _mediatorMock.Setup(m => m.Send(It.IsAny<SetChatAdminCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AccessDeniedException("Назначение администратором", chatId, _testUserId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AccessDeniedException>(
                async () => await _controller.SetChatAdmin(chatId, userId, dto));
            Assert.Equal($"Отказано в доступе для пользователя {_testUserId} при выполнении действия 'Назначение администратором' в чате {chatId}.", exception.Message);
        }

        [Fact]
        public async Task GrantChatAccess_ShouldThrowNotFoundException_WhenRuleNotExists()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var dto = new AccessDto { RoleId = Guid.NewGuid(), Access = ChatAccess.ReadMessages };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GrantChatAccessCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Правило доступа", $"ChatId: {chatId}, RoleId: {dto.RoleId}"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.GrantChatAccess(chatId, dto));
            Assert.Equal($"Сущность \"Правило доступа\" с идентификатором ChatId: {chatId}, RoleId: {dto.RoleId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task RevokeChatAccess_ShouldThrowNotFoundException_WhenRuleNotExists()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var dto = new AccessDto { RoleId = Guid.NewGuid(), Access = ChatAccess.ReadMessages };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RevokeChatAccessCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Правило доступа", $"ChatId: {chatId}, RoleId: {dto.RoleId}"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.RevokeChatAccess(chatId, dto));
            Assert.Equal($"Сущность \"Правило доступа\" с идентификатором ChatId: {chatId}, RoleId: {dto.RoleId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task GetChatInfo_ShouldThrowNotFoundException_WhenChatNotExists()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetChatInfoQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Чат", chatId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.GetChatInfo(chatId));
            Assert.Equal($"Сущность \"Чат\" с идентификатором {chatId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task GetChatInfo_ShouldThrowAccessDeniedException_WhenAccessDenied()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetChatInfoQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AccessDeniedException("Просмотр информации о чате", chatId, _testUserId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AccessDeniedException>(
                async () => await _controller.GetChatInfo(chatId));
            Assert.Equal($"Отказано в доступе для пользователя {_testUserId} при выполнении действия 'Просмотр информации о чате' в чате {chatId}.", exception.Message);
        }

        [Fact]
        public async Task GetChatParticipants_ShouldThrowAccessDeniedException_WhenAccessDenied()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetChatParticipantsQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AccessDeniedException("Просмотр участников чата", chatId, _testUserId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AccessDeniedException>(
                async () => await _controller.GetChatParticipants(chatId));
            Assert.Equal($"Отказано в доступе для пользователя {_testUserId} при выполнении действия 'Просмотр участников чата' в чате {chatId}.", exception.Message);
        }

        [Fact]
        public async Task GetChatAccessRules_ShouldThrowAccessDeniedException_WhenAccessDenied()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetChatAccessRulesQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AccessDeniedException("Просмотр правил доступа", chatId, _testUserId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AccessDeniedException>(
                async () => await _controller.GetChatAccessRules(chatId));
            Assert.Equal($"Отказано в доступе для пользователя {_testUserId} при выполнении действия 'Просмотр правил доступа' в чате {chatId}.", exception.Message);
        }

        [Fact]
        public async Task GetChatActivity_ShouldThrowValidationException_WhenInvalidPagination()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetChatActivityQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("Пагинация", "Take должен быть положительным числом"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                async () => await _controller.GetChatActivity(chatId, skip: 0, take: -1));
            Assert.Equal("Ошибка валидации для поля 'Пагинация': Take должен быть положительным числом", exception.Message);
        }

        #endregion
    }
}