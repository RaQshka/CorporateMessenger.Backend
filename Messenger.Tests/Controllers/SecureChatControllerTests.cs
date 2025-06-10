using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Messenger.Application.SecureChat.Commands;
using Messenger.Application.SecureChat.Queries;
using Messenger.Application.Common.Exceptions;
using Messenger.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Messenger.Tests.Controllers
{
    public class SecureChatControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly SecureChatController _controller;
        private readonly Guid _testUserId = Guid.NewGuid();

        public SecureChatControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new SecureChatController();


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
        

        private Dictionary<string, object> DeserializeResponse(OkObjectResult okResult)
        {
            var json = JsonSerializer.Serialize(okResult.Value);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
        }

        #region Успешные сценарии

        [Fact]
        public async Task Create_ShouldReturnOkWithAccessKeySaltAndPublicKey()
        {
            // Arrange
            var request = new CreateSecureChatRequest { Name = "Secret Chat", InvitedUserId = Guid.NewGuid(), DestroyAt = DateTime.UtcNow.AddDays(1), CreatorPublicKey = new byte[32] };
            var expectedResult = (AccessKey: "test-key", Salt: new byte[16], CreatorPublicKey: request.CreatorPublicKey);
            _mediatorMock.Setup(m => m.Send(It.Is<CreateSecureChatCommand>(c => c.Name == request.Name && c.CreatorId == _testUserId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = DeserializeResponse(okResult);
            Assert.Equal(expectedResult.AccessKey, response["AccessKey"].ToString());
            Assert.Equal(expectedResult.Salt, (byte[]) response["Salt"]);
            Assert.Equal(expectedResult.CreatorPublicKey, (byte[]) response["CreatorPublicKey"]);
        }

        [Fact]
        public async Task Enter_ShouldReturnOkWithChatIdSaltAndOtherPublicKey()
        {
            // Arrange
            var request = new EnterSecureChatRequest { AccessKey = "test-key", PublicKey = new byte[32] };
            var expectedResult = (ChatId: Guid.NewGuid(), Salt: new byte[16], OtherPublicKey: new byte[32]);
            _mediatorMock.Setup(m => m.Send(It.Is<EnterSecureChatCommand>(c => c.AccessKey == request.AccessKey && c.UserId == _testUserId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.Enter(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = DeserializeResponse(okResult);
            Assert.Equal(expectedResult.ChatId, response["ChatId"]);
            Assert.Equal(expectedResult.Salt, response["Salt"]);
            Assert.Equal(expectedResult.OtherPublicKey, response["OtherPublicKey"]);
        }

        [Fact]
        public async Task SendMessage_ShouldReturnNoContent()
        {
            // Arrange
            var request = new SendEncryptedMessageRequest { SecureChatId = Guid.NewGuid(), Ciphertext = new byte[32], IV = new byte[12], Tag = new byte[16] };
            _mediatorMock.Setup(m => m.Send(It.Is<SendEncryptedMessageCommand>(c => c.SecureChatId == request.SecureChatId && c.SenderId == _testUserId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.SendMessage(request);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UploadDocument_ShouldReturnNoContent()
        {
            // Arrange
            var request = new UploadEncryptedDocumentRequest { SecureChatId = Guid.NewGuid(), FileData = new byte[1024], IV = new byte[12], Tag = new byte[16], FileName = "test.pdf", FileType = "pdf" };
            _mediatorMock.Setup(m => m.Send(It.Is<UploadEncryptedDocumentCommand>(c => c.SecureChatId == request.SecureChatId && c.UploaderId == _testUserId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.UploadDocument(request);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Destroy_ShouldReturnNoContent()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.Is<DestroySecureChatCommand>(c => c.SecureChatId == chatId && c.UserId == _testUserId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.Destroy(chatId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Activity_ShouldReturnOkWithActivityList()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var expectedActivity = new List<SecureChatActivityDto>
            {
                new SecureChatActivityDto { Id = Guid.NewGuid(), Type = "Message", Timestamp = DateTime.UtcNow },
                new SecureChatActivityDto { Id = Guid.NewGuid(), Type = "Document", Timestamp = DateTime.UtcNow }
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetSecureChatActivityQuery>(q => q.ChatId == chatId && q.UserId == _testUserId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedActivity);

            // Act
            var result = await _controller.Activity(chatId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedActivity, okResult.Value);
        }

        #endregion

        #region Сценарии с исключениями

        [Fact]
        public async Task Create_ShouldThrowValidationException_WhenNameIsEmpty()
        {
            // Arrange
            var request = new CreateSecureChatRequest { Name = "", InvitedUserId = Guid.NewGuid(), DestroyAt = DateTime.UtcNow.AddDays(1), CreatorPublicKey = new byte[32] };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateSecureChatCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("name", "Название чата не может быть пустым"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                async () => await _controller.Create(request));
            Assert.Equal("Ошибка валидации для поля 'name': Название чата не может быть пустым", exception.Message);
        }

        [Fact]
        public async Task Enter_ShouldThrowNotFoundException_WhenChatNotFound()
        {
            // Arrange
            var request = new EnterSecureChatRequest { AccessKey = "invalid-key", PublicKey = new byte[32] };
            _mediatorMock.Setup(m => m.Send(It.IsAny<EnterSecureChatCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("SecureChat", "invalid-key"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.Enter(request));
            Assert.Equal("Сущность \"SecureChat\" с идентификатором invalid-key не найдена.", exception.Message);
        }

        [Fact]
        public async Task SendMessage_ShouldThrowBusinessRuleException_WhenChatExpired()
        {
            // Arrange
            var request = new SendEncryptedMessageRequest { SecureChatId = Guid.NewGuid(), Ciphertext = new byte[32], IV = new byte[12], Tag = new byte[16] };
            _mediatorMock.Setup(m => m.Send(It.IsAny<SendEncryptedMessageCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BusinessRuleException("Срок действия чата истек"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                async () => await _controller.SendMessage(request));
            Assert.Equal("Нарушение бизнес-правила: Срок действия чата истек", exception.Message);
        }

        [Fact]
        public async Task UploadDocument_ShouldThrowAccessDeniedException_WhenNotParticipant()
        {
            // Arrange
            var request = new UploadEncryptedDocumentRequest { SecureChatId = Guid.NewGuid(), FileData = new byte[1024], IV = new byte[12], Tag = new byte[16], FileName = "test.pdf", FileType = "pdf" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UploadEncryptedDocumentCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AccessDeniedException("загрузка документа", request.SecureChatId, _testUserId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AccessDeniedException>(
                async () => await _controller.UploadDocument(request));
            Assert.Equal($"Отказано в доступе для пользователя {_testUserId} при выполнении действия 'загрузка документа' в чате {request.SecureChatId}.", exception.Message);
        }

        [Fact]
        public async Task Destroy_ShouldThrowNotFoundException_WhenChatNotFound()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DestroySecureChatCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("SecureChat", chatId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.Destroy(chatId));
            Assert.Equal($"Сущность \"SecureChat\" с идентификатором {chatId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task Activity_ShouldThrowAccessDeniedException_WhenNotParticipant()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetSecureChatActivityQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AccessDeniedException("получение активности", chatId, _testUserId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AccessDeniedException>(
                async () => await _controller.Activity(chatId));
            Assert.Equal($"Отказано в доступе для пользователя {_testUserId} при выполнении действия 'получение активности' в чате {chatId}.", exception.Message);
        }

        #endregion
    }
}