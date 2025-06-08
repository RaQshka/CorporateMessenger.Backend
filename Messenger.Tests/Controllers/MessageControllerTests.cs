using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Messenger.Application.Messages.Commands.AddReaction;
using Messenger.Application.Messages.Commands.DeleteMessage;
using Messenger.Application.Messages.Commands.EditMessage;
using Messenger.Application.Messages.Commands.RemoveReaction;
using Messenger.Application.Messages.Commands.SendMessage;
using Messenger.Application.Messages.Queries.GetMessages;
using Messenger.Application.Messages.Queries.GetReactions;
using Messenger.Application.Messages.Queries.Shared;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Enums;
using Messenger.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Messenger.Tests.Controllers
{
    public class MessagesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IAuditLogger> _auditLoggerMock;
        private readonly MessagesController _controller;
        private readonly Guid _testUserId = Guid.NewGuid();

        public MessagesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _auditLoggerMock = new Mock<IAuditLogger>();
            _controller = new MessagesController(_auditLoggerMock.Object);

            // Настройка аутентификации
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString())
            }, "mock"));

            // Настройка IServiceProvider для возврата _mediatorMock
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IMediator)))
                .Returns(_mediatorMock.Object);

            // Настройка HttpContext
            var httpContext = new DefaultHttpContext
            {
                User = user,
                RequestServices = serviceProviderMock.Object
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        private Dictionary<string, string> DeserializeResponse(OkObjectResult okResult)
        {
            var json = JsonSerializer.Serialize(okResult.Value);
            var response = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            return response ?? new Dictionary<string, string>();
        }

        #region Успешные сценарии

        [Fact]
        public async Task SendMessage_ShouldReturnOkWithMessageId()
        {
            // Arrange
            var command = new SendMessageCommand { ChatId = Guid.NewGuid(), Content = "Hello" };
            var expectedMessageId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.Is<SendMessageCommand>(c => c.SenderId == _testUserId &&
                                                                           c.ChatId == command.ChatId &&
                                                                           c.Content == command.Content),
                                           It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedMessageId);

            // Act
            var result = await _controller.SendMessage(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = DeserializeResponse(okResult);
            Assert.Equal(expectedMessageId.ToString(), response["MessageId"]);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "Отправка сообщения", "Сообщение", expectedMessageId,
                "Сообщение отправлено", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task EditMessage_ShouldReturnNoContent()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            var request = new EditMessageRequest { NewContent = "Updated Content" };
            _mediatorMock.Setup(m => m.Send(It.Is<EditMessageCommand>(c => c.MessageId == messageId &&
                                                                           c.UserId == _testUserId &&
                                                                           c.NewContent == request.NewContent),
                                           It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.EditMessage(messageId, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "Редактирование сообщения", "Сообщение", messageId,
                "Сообщение отредактировано", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task DeleteMessage_ShouldReturnNoContent()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.Is<DeleteMessageCommand>(c => c.MessageId == messageId &&
                                                                             c.UserId == _testUserId),
                                           It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.DeleteMessage(messageId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "Удаление сообщения", "Сообщение", messageId,
                "Сообщение удалено", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task AddReaction_ShouldReturnNoContent()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            var request = new AddReactionRequest { ReactionType = "Like" };
            _mediatorMock.Setup(m => m.Send(It.Is<AddReactionCommand>(c => c.MessageId == messageId &&
                                                                           c.UserId == _testUserId &&
                                                                           c.ReactionType == ReactionType.Like),
                                           It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.AddReaction(messageId, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "Добавление реакции", "Реакция", messageId,
                "Реакция добавлена", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task RemoveReaction_ShouldReturnNoContent()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.Is<RemoveReactionCommand>(c => c.MessageId == messageId &&
                                                                              c.UserId == _testUserId),
                                           It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.RemoveReaction(messageId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "Удаление реакции", "Реакция", messageId,
                "Реакция удалена", LogLevel.Info), Times.Once());
        }
/*
        [Fact]
        public async Task GetMessages_ShouldReturnOkWithMessages()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var expectedMessages = new List<MessageDto>
            {
                new MessageDto { Id = Guid.NewGuid(), Content = "Test Message", ChatId = chatId, SenderId = _testUserId }
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetMessagesQuery>(q => q.ChatId == chatId &&
                                                                         q.UserId == _testUserId &&
                                                                         q.Skip == 0 &&
                                                                         q.Take == 20),
                                           It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedMessages);

            // Act
            var result = await _controller.GetMessages(chatId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedMessages, okResult.Value);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "Получение сообщений", "Чат", chatId,
                "Сообщения получены", LogLevel.Info), Times.Once());
        }*/

        [Fact]
        public async Task GetReactions_ShouldReturnOkWithReactions()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            var expectedReactions = new List<MessageReactionDto>
            {
                new MessageReactionDto { Id = Guid.NewGuid(), ReactionType = ReactionType.Like, UserId = _testUserId }
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetReactionsQuery>(q => q.MessageId == messageId &&
                                                                          q.UserId == _testUserId),
                                           It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedReactions);

            // Act
            var result = await _controller.GetReactions(messageId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedReactions, okResult.Value);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "Получение реакций", "Сообщение", messageId,
                "Реакции получены", LogLevel.Info), Times.Once());
        }

        #endregion

        #region Сценарии с исключениями

        [Fact]
        public async Task SendMessage_ShouldThrowNotFoundException_WhenChatNotExists()
        {
            // Arrange
            var command = new SendMessageCommand { ChatId = Guid.NewGuid(), Content = "Hello" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<SendMessageCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException("Чат", command.ChatId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.SendMessage(command));
            Assert.Equal($"Сущность \"Чат\" с идентификатором {command.ChatId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task SendMessage_ShouldThrowBusinessRuleException_WhenUserNotParticipant()
        {
            // Arrange
            var command = new SendMessageCommand { ChatId = Guid.NewGuid(), Content = "Hello" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<SendMessageCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new BusinessRuleException("Пользователь не является участником чата"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                async () => await _controller.SendMessage(command));
            Assert.Equal("Нарушение бизнес-правила: Пользователь не является участником чата", exception.Message);
        }

        [Fact]
        public async Task SendMessage_ShouldThrowBusinessRuleException_WhenNoWritePermission()
        {
            // Arrange
            var command = new SendMessageCommand { ChatId = Guid.NewGuid(), Content = "Hello" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<SendMessageCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new BusinessRuleException("У пользователя нет прав на отправку сообщений в этот чат"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                async () => await _controller.SendMessage(command));
            Assert.Equal("Нарушение бизнес-правила: У пользователя нет прав на отправку сообщений в этот чат", exception.Message);
        }

        [Fact]
        public async Task EditMessage_ShouldThrowNotFoundException_WhenMessageNotExists()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            var request = new EditMessageRequest { NewContent = "Updated Content" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<EditMessageCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException("Сообщение", messageId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.EditMessage(messageId, request));
            Assert.Equal($"Сущность \"Сообщение\" с идентификатором {messageId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task EditMessage_ShouldThrowBusinessRuleException_WhenUserNotAllowed()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            var request = new EditMessageRequest { NewContent = "Updated Content" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<EditMessageCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new BusinessRuleException("Только отправитель или администратор могут редактировать сообщение"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                async () => await _controller.EditMessage(messageId, request));
            Assert.Equal("Нарушение бизнес-правила: Только отправитель или администратор могут редактировать сообщение", exception.Message);
        }

        [Fact]
        public async Task DeleteMessage_ShouldThrowNotFoundException_WhenMessageNotExists()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteMessageCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException("Сообщение", messageId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.DeleteMessage(messageId));
            Assert.Equal($"Сущность \"Сообщение\" с идентификатором {messageId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task DeleteMessage_ShouldThrowBusinessRuleException_WhenUserNotAllowed()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteMessageCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new BusinessRuleException("Только отправитель или администратор могут удалить сообщение"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                async () => await _controller.DeleteMessage(messageId));
            Assert.Equal("Нарушение бизнес-правила: Только отправитель или администратор могут удалить сообщение", exception.Message);
        }

        [Fact]
        public async Task AddReaction_ShouldThrowBusinessRuleException_WhenReactionAlreadyExists()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            var request = new AddReactionRequest { ReactionType = "Like" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddReactionCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new BusinessRuleException("Пользователь уже добавил реакцию к этому сообщению"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                async () => await _controller.AddReaction(messageId, request));
            Assert.Equal("Нарушение бизнес-правила: Пользователь уже добавил реакцию к этому сообщению", exception.Message);
        }

        [Fact]
        public async Task AddReaction_ShouldReturnNotFound_WhenReactionTypeInvalid()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            var request = new AddReactionRequest { ReactionType = "Invalid" };

            // Act
            var result = await _controller.AddReaction(messageId, request);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoveReaction_ShouldThrowNotFoundException_WhenReactionNotExists()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveReactionCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException("Реакция", "не найдена"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.RemoveReaction(messageId));
            Assert.Equal("Сущность \"Реакция\" с идентификатором не найдена не найдена.", exception.Message);
        }

        [Fact]
        public async Task GetMessages_ShouldThrowNotFoundException_WhenChatNotExists()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMessagesQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException("Чат", chatId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.GetMessages(chatId));
            Assert.Equal($"Сущность \"Чат\" с идентификатором {chatId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task GetMessages_ShouldThrowBusinessRuleException_WhenNoReadPermission()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMessagesQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new BusinessRuleException("У пользователя нет прав на чтение сообщений в этом чате"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                async () => await _controller.GetMessages(chatId));
            Assert.Equal("Нарушение бизнес-правила: У пользователя нет прав на чтение сообщений в этом чате", exception.Message);
        }

        [Fact]
        public async Task GetReactions_ShouldThrowNotFoundException_WhenMessageNotExists()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetReactionsQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException("Сообщение", messageId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.GetReactions(messageId));
            Assert.Equal($"Сущность \"Сообщение\" с идентификатором {messageId} не найдена.", exception.Message);
        }

        #endregion
    }
}