using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Messenger.Application.Documents.Commands.DeleteDocument;
using Messenger.Application.Documents.Commands.GrantDocumentAccess;
using Messenger.Application.Documents.Commands.RevokeDocumentAccess;
using Messenger.Application.Documents.Commands.UploadDocument;
using Messenger.Application.Documents.Queries.DownloadDocument;
using Messenger.Application.Documents.Queries.GetDocumentAccessRules;
using Messenger.Application.Documents.Queries.GetDocuments;
using Messenger.Application.Documents.Queries.Shared;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Enums;
using Messenger.WebApi.Controllers;
using Messenger.WebApi.Hubs;
using Messenger.WebApi.Models.MessageDtos.Messenger.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Messenger.Tests.Controllers
{
    public class DocumentsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ChatHub> _hubMock;
        private readonly Mock<IHubContext<ChatHub>> _hubContextMock;
        private readonly Mock<IAuditLogger> _auditLoggerMock;
        private readonly DocumentsController _controller;
        private readonly Guid _testUserId = Guid.NewGuid();

        public DocumentsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _auditLoggerMock = new Mock<IAuditLogger>();
            _controller = new DocumentsController(_auditLoggerMock.Object);
            _hubMock = new Mock<ChatHub>(_mediatorMock.Object);
            _hubContextMock = new Mock<IHubContext<ChatHub>>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString())
            }, "mock"));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IMediator)))
                .Returns(_mediatorMock.Object);
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IHubContext<ChatHub>)))
                .Returns(_hubContextMock.Object);

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
        public async Task UploadDocument_ShouldReturnOkWithDocumentId_WhenValidFileUploaded()
        {
            // Arrange
            var command = new UploadDocumentCommand { ChatId = Guid.NewGuid(), File = new Mock<IFormFile>().Object };
            var expectedDocumentId = Guid.NewGuid();

            // Настройка Mediator
            _mediatorMock.Setup(m => m.Send(It.Is<UploadDocumentCommand>
                    (c => c.UploaderId == _testUserId &&
                      c.ChatId == command.ChatId &&
                      c.File == command.File),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDocumentId);
            
            // Act
            var result = await _controller.UploadDocument(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = DeserializeResponse(okResult);
            Assert.Equal(expectedDocumentId.ToString(), response["DocumentId"]);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "Загрузка документа", "Документ", expectedDocumentId,
                "Документ загружен", LogLevel.Info), Times.Once());
        }
        [Fact]
        public async Task DownloadDocument_ShouldReturnFile_WhenUserHasViewPermission()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var query = new DownloadDocumentQuery { DocumentId = documentId, UserId = _testUserId };
            var expectedFileContent = new byte[] { 0x01, 0x02, 0x03 };
            var expectedFileName = "test.pdf";
            var expectedContentType = "application/pdf";
            _mediatorMock.Setup(m => m.Send(It.Is<DownloadDocumentQuery>(q => q.DocumentId == documentId && q.UserId == _testUserId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DocumentDownloadDto
                {
                    Content = expectedFileContent,
                    FileName = expectedFileName,
                    ContentType = expectedContentType
                });

            // Act
            var result = await _controller.DownloadDocument(documentId);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal(expectedFileContent, fileResult.FileContents);
            Assert.Equal(expectedFileName, fileResult.FileDownloadName);
            Assert.Equal(expectedContentType, fileResult.ContentType);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "Скачивание документа", "Документ", documentId,
                "Документ скачан", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task DeleteDocument_ShouldReturnNoContent_WhenUserHasDeletePermission()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.Is<DeleteDocumentCommand>(c => c.DocumentId == documentId &&
                                                                              c.UserId == _testUserId),
                                           It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.DeleteDocument(documentId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "Удаление документа", "Документ", documentId,
                "Документ удален", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task GrantDocumentAccess_ShouldReturnNoContent_WhenCreatorGrantsAccess()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var request = new DocumentAccessRequest { RoleId = Guid.NewGuid(), AccessFlag = DocumentAccess.ViewDocument };
            _mediatorMock.Setup(m => m.Send(It.Is<GrantDocumentAccessCommand>(c => c.DocumentId == documentId &&
                                                                                   c.InitiatorId == _testUserId &&
                                                                                   c.RoleId == request.RoleId &&
                                                                                   c.AccessFlag == request.AccessFlag),
                                           It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.GrantDocumentAccess(documentId, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "Предоставление доступа", "Документ", documentId,
                "Доступ предоставлен", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task RevokeDocumentAccess_ShouldReturnNoContent_WhenCreatorRevokesAccess()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var request = new DocumentAccessRequest { RoleId = Guid.NewGuid(), AccessFlag = DocumentAccess.ViewDocument };
            _mediatorMock.Setup(m => m.Send(It.Is<RevokeDocumentAccessCommand>(c => c.DocumentId == documentId &&
                                                                                    c.InitiatorId == _testUserId &&
                                                                                    c.RoleId == request.RoleId &&
                                                                                    c.AccessFlag == request.AccessFlag),
                                           It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.RevokeDocumentAccess(documentId, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "Отзыв доступа", "Документ", documentId,
                "Доступ отозван", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task GetDocuments_ShouldReturnOkWithDocuments_WhenUserHasReadPermission()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var expectedDocuments = new List<DocumentDto>
            {
                new DocumentDto { Id = Guid.NewGuid(), ChatId = chatId, FileName = "test.pdf" }
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetDocumentsQuery>(q => q.ChatId == chatId &&
                                                                          q.UserId == _testUserId),
                                           It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedDocuments);

            // Act
            var result = await _controller.GetDocuments(chatId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedDocuments, okResult.Value);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "Получение документов", "Чат", chatId,
                "Документы получены", LogLevel.Info), Times.Once());
        }

        [Fact]
        public async Task GetDocumentAccessRules_ShouldReturnOkWithRules_WhenUserIsAdmin()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var expectedRules = new List<DocumentAccessRuleDto>
            {
                new DocumentAccessRuleDto { Id = Guid.NewGuid(), DocumentId = documentId, RoleId = Guid.NewGuid(), DocumentAccessMask = (int)DocumentAccess.ViewDocument }
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetDocumentAccessRulesQuery>(q => q.DocumentId == documentId &&
                                                                                    q.UserId == _testUserId),
                                           It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedRules);

            // Act
            var result = await _controller.GetDocumentAccessRules(documentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedRules, okResult.Value);
            _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "Получение правил доступа", "Документ", documentId,
                "Правила доступа получены", LogLevel.Info), Times.Once());
        }

        #endregion

        #region Сценарии с исключениями

        // UploadDocument

        [Fact]
        public async Task UploadDocument_ShouldThrowNotFoundException_WhenChatNotExists()
        {
            // Arrange
            var command = new UploadDocumentCommand { ChatId = Guid.NewGuid(), File = new Mock<IFormFile>().Object };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UploadDocumentCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException("Чат", command.ChatId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.UploadDocument(command));
            Assert.Equal($"Сущность \"Чат\" с идентификатором {command.ChatId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task UploadDocument_ShouldThrowBusinessRuleException_WhenUserNotParticipant()
        {
            // Arrange
            var command = new UploadDocumentCommand { ChatId = Guid.NewGuid(), File = new Mock<IFormFile>().Object };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UploadDocumentCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new BusinessRuleException("Пользователь не является участником чата"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                async () => await _controller.UploadDocument(command));
            Assert.Equal("Нарушение бизнес-правила: Пользователь не является участником чата", exception.Message);
        }

        [Fact]
        public async Task UploadDocument_ShouldThrowBusinessRuleException_WhenNoWritePermission()
        {
            // Arrange
            var command = new UploadDocumentCommand { ChatId = Guid.NewGuid(), File = new Mock<IFormFile>().Object };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UploadDocumentCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new BusinessRuleException("У пользователя нет прав на загрузку документов в этот чат"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                async () => await _controller.UploadDocument(command));
            Assert.Equal("Нарушение бизнес-правила: У пользователя нет прав на загрузку документов в этот чат", exception.Message);
        }

        [Fact]
        public async Task UploadDocument_ShouldThrowValidationException_WhenFileIsNull()
        {
            // Arrange
            var command = new UploadDocumentCommand { ChatId = Guid.NewGuid(), File = null };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UploadDocumentCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new ValidationException("Файл", "Файл не предоставлен"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                async () => await _controller.UploadDocument(command));
            Assert.Equal("Ошибка валидации для поля 'Файл': Файл не предоставлен", exception.Message);
        }

        [Fact]
        public async Task UploadDocument_ShouldThrowValidationException_WhenFileSizeExceedsLimit()
        {
            // Arrange
            var command = new UploadDocumentCommand { ChatId = Guid.NewGuid(), File = new Mock<IFormFile>().Object };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UploadDocumentCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new ValidationException("Файл", "Размер файла превышает допустимый лимит (15 МБ)"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                async () => await _controller.UploadDocument(command));
            Assert.Equal("Ошибка валидации для поля 'Файл': Размер файла превышает допустимый лимит (15 МБ)", exception.Message);
        }

        [Fact]
        public async Task UploadDocument_ShouldThrowValidationException_WhenFileTypeInvalid()
        {
            // Arrange
            var command = new UploadDocumentCommand { ChatId = Guid.NewGuid(), File = new Mock<IFormFile>().Object };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UploadDocumentCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new ValidationException("Файл", "Недопустимый тип файла. Допустимые форматы: .pdf, .pptx, .doc, .docx, .rtf, .jpg, .jpeg, .png, .gif"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                async () => await _controller.UploadDocument(command));
            Assert.Equal("Ошибка валидации для поля 'Файл': Недопустимый тип файла. Допустимые форматы: .pdf, .pptx, .doc, .docx, .rtf, .jpg, .jpeg, .png, .gif", exception.Message);
        }

        [Fact]
        public async Task UploadDocument_ShouldThrowBusinessRuleException_WhenFileSaveFails()
        {
            // Arrange
            var command = new UploadDocumentCommand { ChatId = Guid.NewGuid(), File = new Mock<IFormFile>().Object };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UploadDocumentCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new BusinessRuleException("Ошибка при сохранении файла: Access denied"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                async () => await _controller.UploadDocument(command));
            Assert.Equal("Нарушение бизнес-правила: Ошибка при сохранении файла: Access denied", exception.Message);
        }

        // DownloadDocument

        [Fact]
        public async Task DownloadDocument_ShouldThrowNotFoundException_WhenDocumentNotExists()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DownloadDocumentQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException("Документ", documentId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.DownloadDocument(documentId));
            Assert.Equal($"Сущность \"Документ\" с идентификатором {documentId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task DownloadDocument_ShouldThrowAccessDeniedException_WhenNoViewPermission()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DownloadDocumentQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new AccessDeniedException("Скачивание документа", Guid.NewGuid(), _testUserId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AccessDeniedException>(
                async () => await _controller.DownloadDocument(documentId));
            Assert.Contains("Отказано в доступе для пользователя", exception.Message);
        }

        [Fact]
        public async Task DownloadDocument_ShouldThrowBusinessRuleException_WhenFileReadFails()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DownloadDocumentQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new BusinessRuleException("Ошибка при чтении файла: File not found"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                async () => await _controller.DownloadDocument(documentId));
            Assert.Equal("Нарушение бизнес-правила: Ошибка при чтении файла: File not found", exception.Message);
        }

        // DeleteDocument

        [Fact]
        public async Task DeleteDocument_ShouldThrowNotFoundException_WhenDocumentNotExists()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteDocumentCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException("Документ", documentId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.DeleteDocument(documentId));
            Assert.Equal($"Сущность \"Документ\" с идентификатором {documentId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task DeleteDocument_ShouldThrowAccessDeniedException_WhenNoDeletePermission()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteDocumentCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new AccessDeniedException("Удаление документа", Guid.NewGuid(), _testUserId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AccessDeniedException>(
                async () => await _controller.DeleteDocument(documentId));
            Assert.Contains("Отказано в доступе для пользователя", exception.Message);
        }

        [Fact]
        public async Task DeleteDocument_ShouldThrowBusinessRuleException_WhenFileDeleteFails()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteDocumentCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new BusinessRuleException("Ошибка при удалении файла: Permission denied"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                async () => await _controller.DeleteDocument(documentId));
            Assert.Equal("Нарушение бизнес-правила: Ошибка при удалении файла: Permission denied", exception.Message);
        }

        // GrantDocumentAccess

        [Fact]
        public async Task GrantDocumentAccess_ShouldThrowNotFoundException_WhenDocumentNotExists()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var request = new DocumentAccessRequest { RoleId = Guid.NewGuid(), AccessFlag = DocumentAccess.ViewDocument };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GrantDocumentAccessCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException("Документ", documentId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.GrantDocumentAccess(documentId, request));
            Assert.Equal($"Сущность \"Документ\" с идентификатором {documentId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task GrantDocumentAccess_ShouldThrowAccessDeniedException_WhenNotCreator()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var request = new DocumentAccessRequest { RoleId = Guid.NewGuid(), AccessFlag = DocumentAccess.ViewDocument };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GrantDocumentAccessCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new AccessDeniedException("Предоставление доступа к документу", Guid.NewGuid(), _testUserId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AccessDeniedException>(
                async () => await _controller.GrantDocumentAccess(documentId, request));
            Assert.Contains("Отказано в доступе для пользователя", exception.Message);
        }

        [Fact]
        public async Task GrantDocumentAccess_ShouldThrowNotFoundException_WhenRoleNotExists()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var request = new DocumentAccessRequest { RoleId = Guid.NewGuid(), AccessFlag = DocumentAccess.ViewDocument };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GrantDocumentAccessCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException("Роль", request.RoleId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.GrantDocumentAccess(documentId, request));
            Assert.Equal($"Сущность \"Роль\" с идентификатором {request.RoleId} не найдена.", exception.Message);
        }

        // RevokeDocumentAccess

        [Fact]
        public async Task RevokeDocumentAccess_ShouldThrowNotFoundException_WhenDocumentNotExists()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var request = new DocumentAccessRequest() { RoleId = Guid.NewGuid(), AccessFlag = DocumentAccess.ViewDocument };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RevokeDocumentAccessCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException("Документ", documentId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.RevokeDocumentAccess(documentId, request));
            Assert.Equal($"Сущность \"Документ\" с идентификатором {documentId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task RevokeDocumentAccess_ShouldThrowAccessDeniedException_WhenNotCreator()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var request = new DocumentAccessRequest { RoleId = Guid.NewGuid(), AccessFlag = DocumentAccess.ViewDocument };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RevokeDocumentAccessCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new AccessDeniedException("Отзыв доступа к документу", Guid.NewGuid(), _testUserId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AccessDeniedException>(
                async () => await _controller.RevokeDocumentAccess(documentId, request));
            Assert.Contains("Отказано в доступе для пользователя", exception.Message);
        }

        [Fact]
        public async Task RevokeDocumentAccess_ShouldThrowNotFoundException_WhenRoleNotExists()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var request = new DocumentAccessRequest { RoleId = Guid.NewGuid(), AccessFlag = DocumentAccess.ViewDocument };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RevokeDocumentAccessCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException("Роль", request.RoleId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.RevokeDocumentAccess(documentId, request));
            Assert.Equal($"Сущность \"Роль\" с идентификатором {request.RoleId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task RevokeDocumentAccess_ShouldThrowNotFoundException_WhenRuleNotExists()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var request = new DocumentAccessRequest { RoleId = Guid.NewGuid(), AccessFlag = DocumentAccess.ViewDocument };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RevokeDocumentAccessCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException("Правило доступа", $"DocumentId: {documentId}, RoleId: {request.RoleId}"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.RevokeDocumentAccess(documentId, request));
            Assert.Equal($"Сущность \"Правило доступа\" с идентификатором DocumentId: {documentId}, RoleId: {request.RoleId} не найдена.", exception.Message);
        }

        // GetDocuments

        [Fact]
        public async Task GetDocuments_ShouldThrowNotFoundException_WhenChatNotExists()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDocumentsQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException("Чат", chatId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.GetDocuments(chatId));
            Assert.Equal($"Сущность \"Чат\" с идентификатором {chatId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task GetDocuments_ShouldThrowAccessDeniedException_WhenNoReadPermission()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDocumentsQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new AccessDeniedException("Просмотр документов", chatId, _testUserId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AccessDeniedException>(
                async () => await _controller.GetDocuments(chatId));
            Assert.Contains("Отказано в доступе для пользователя", exception.Message);
        }

        // GetDocumentAccessRules

        [Fact]
        public async Task GetDocumentAccessRules_ShouldThrowNotFoundException_WhenDocumentNotExists()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDocumentAccessRulesQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException("Документ", documentId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _controller.GetDocumentAccessRules(documentId));
            Assert.Equal($"Сущность \"Документ\" с идентификатором {documentId} не найдена.", exception.Message);
        }

        [Fact]
        public async Task GetDocumentAccessRules_ShouldThrowAccessDeniedException_WhenNotAdmin()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDocumentAccessRulesQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new AccessDeniedException("Просмотр правил доступа", Guid.NewGuid(), _testUserId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AccessDeniedException>(
                async () => await _controller.GetDocumentAccessRules(documentId));
            Assert.Contains("Отказано в доступе для пользователя", exception.Message);
        }

        #endregion
    }
}