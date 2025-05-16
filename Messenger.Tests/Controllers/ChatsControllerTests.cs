using System.Security.Claims;
using MediatR;
using Messenger.Application.Chats.Commands.CreateChat;
using Messenger.Application.Chats.Commands.DeleteChat;
using Messenger.Application.Chats.Queries.GetUserChats;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Messenger.WebApi.Controllers;
using Messenger.WebApi.Models.ChatDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
/*
public class ChatsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IAuditLogger> _auditLoggerMock;
    private readonly ChatsController _controller;
    private readonly Guid _testUserId = Guid.NewGuid();

    public ChatsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _auditLoggerMock = new Mock<IAuditLogger>();
        _controller = new ChatsController(_mediatorMock.Object, _auditLoggerMock.Object);

        // Настройка HttpContext с тестовым пользователем
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString())
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetUserChats_ShouldReturnOkWithChats()
    {
        // Arrange
        var expectedChats = new List<Chat> { new Chat { Id = Guid.NewGuid(), ChatName = "Test Chat" } };
        _mediatorMock.Setup(m => m.Send(It.Is<GetUserChatsQuery>(q => q.UserId == _testUserId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedChats);

        // Act
        var result = await _controller.GetUserChats();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedChats, okResult.Value);
        _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "GetUserChats", "Chat", _testUserId,
            "Список чатов пользователя успешно получен"), Times.Once());
    }

    [Fact]
    public async Task CreateChat_ShouldReturnOkWithChatId()
    {
        // Arrange
        var dto = new CreateChatDto { Name = "New Chat", Type = ChatTypes.Chat };
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
        var response = Assert.IsType<dynamic>(okResult.Value);
        Assert.Equal(expectedChatId, response.ChatId);
        _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "CreateChat", "Chat", expectedChatId,
            "Чат успешно создан"), Times.Once());
    }

    [Fact]
    public async Task DeleteChat_ShouldReturnOk()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.Is<DeleteChatCommand>(c => c.ChatId == chatId &&
                                                                      c.InitiatorId == _testUserId),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteChat(chatId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<dynamic>(okResult.Value);
        Assert.Equal("Чат успешно удален", response.message);
        _auditLoggerMock.Verify(l => l.LogAsync(_testUserId, "DeleteChat", "Chat", chatId,
            "Чат успешно удален"), Times.Once());
    }
}*/