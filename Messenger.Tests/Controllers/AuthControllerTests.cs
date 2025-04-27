using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Messenger.Application.Interfaces;
using Messenger.Application.Users.Commands.AssignRole;
using Messenger.Application.Users.Commands.ConfirmEmail;
using Messenger.Application.Users.Commands.LoginUser;
using Messenger.Application.Users.Commands.LogoutUser;
using Messenger.Application.Users.Commands.RefreshToken;
using Messenger.Application.Users.Commands.RegisterUser;
using Messenger.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Messenger.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IAuditLogger> _auditMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new AuthController(_mediatorMock.Object, _auditMock.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnOk()
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                Username = "test", 
                Email = "test@example.com", 
                Password = "Test@123"
            };
            _mediatorMock.Setup(m => 
                    m.Send(command, 
                        It.IsAny<CancellationToken>())
                    ).ReturnsAsync(new RegistrationResult(){ Message = "Регистрация успешна." });

            // Act
            var result = await _controller.Register(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task ConfirmEmail_ShouldReturnOk_WhenEmailConfirmed()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "testToken";
            _mediatorMock.Setup(m => m.Send(It.Is<ConfirmEmailCommand>(c => c.UserId == userId && c.Token == token),
                                             It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            // Act
            var result = await _controller.ConfirmEmail(userId, token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Email подтверждён.", okResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenInvalidCredentials()
        {
            // Arrange
            var command = new LoginUserCommand { Username = "test", Password = "wrongpassword" };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new LoginResult { Message = "Ошибка входа." });

            // Act
            var result = await _controller.Login(command);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Ошибка входа.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Logout_ShouldReturnOk_WhenAuthorized()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(
                    It.IsAny<LogoutUserCommand>(), 
                    It.IsAny<CancellationToken>()
                    )).ReturnsAsync(new LogoutUserResult()
                    {
                        Message = "Вы успешно вышли из системы", 
                        Success = true
                    });

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await _controller.Logout();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Вы успешно вышли из системы.", okResult.Value);
        }

        [Fact]
        public async Task AssignRole_ShouldReturnOk_WhenAdmin()
        {
            // Arrange
            var command = new AssignRoleCommand { UserId = Guid.NewGuid(), Role = "Admin" };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new AssignRoleResult(){Success = true, Message = "Роль добавлена."});

            // Act
            var result = await _controller.AssignRole(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Refresh_ShouldReturnUnauthorized_WhenTokenIsMissing()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = await _controller.Refresh();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            //получение из анонимного типа строку message
            var dict = new Dictionary<string, object>();
            var val = unauthorizedResult.Value;
            dict["message"] = val.GetType().GetProperties()[0].GetValue(val);
            string message = dict["message"] as string;
            //сравнение результатов
            Assert.Equal("Токен отсутствует", message);
        }
    }
}
