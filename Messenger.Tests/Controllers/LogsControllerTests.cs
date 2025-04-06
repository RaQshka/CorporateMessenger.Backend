using MediatR;
using Messenger.Application.AuditLogs.Queries.ExportUserAuditLogs;
using Messenger.Application.AuditLogs.Queries.GetUserAuditLogs;
using Messenger.Application.AuditLogs.Queries.Shared;
using Messenger.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Messenger.Tests.Controllers
{
    public class LogsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly LogsController _controller;

        public LogsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new LogsController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetAuditLogs_ShouldReturnOk_WithValidRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var testLogs = new List<AuditLogDto>
            {
                new() { LogID = Guid.NewGuid(), UserID = userId }
            };

            _mediatorMock.Setup(m => m.Send(
                It.Is<GetUserAuditLogsQuery>(q => 
                    q.UserId == userId && 
                    q.Days == null && 
                    q.StartTime == null && 
                    q.EndDate == null),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(testLogs);

            // Act
            var result = await _controller.GetAuditLogsAsync(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedLogs = Assert.IsType<List<AuditLogDto>>(okResult.Value);
            Assert.Single(returnedLogs);
        }

        [Fact]
        public async Task GetAuditLogs_ShouldApplyDateFilters_WhenProvided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(-7);
            var endDate = DateTime.UtcNow;

            _mediatorMock.Setup(m => m.Send(
                It.Is<GetUserAuditLogsQuery>(q => 
                    q.UserId == userId &&
                    q.StartTime == startDate &&
                    q.EndDate == endDate),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AuditLogDto>());

            // Act
            var result = await _controller.GetAuditLogsAsync(
                userId, 
                startTime: startDate, 
                endDate: endDate);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ExportUserAudit_ShouldReturnFileResult_WithValidRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var testStream = new MemoryStream();
            var writer = new StreamWriter(testStream);
            await writer.WriteLineAsync("test,data");
            await writer.FlushAsync();
            testStream.Position = 0;

            _mediatorMock.Setup(m => m.Send(
                It.Is<ExportUserAuditLogsQuery>(q => q.UserId == userId),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(testStream);

            // Act
            var result = await _controller.ExportUserAuditAsync(userId);

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Contains(userId.ToString(), fileResult.FileDownloadName);
        }

        [Fact]
        public async Task ExportUserAudit_ShouldApplyDateFilters_WhenProvided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var days = 14;
            var testStream = new MemoryStream();

            _mediatorMock.Setup(m => m.Send(
                It.Is<ExportUserAuditLogsQuery>(q => 
                    q.UserId == userId &&
                    q.Days == days),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(testStream);

            // Act
            var result = await _controller.ExportUserAuditAsync(
                userId, 
                days: days);

            // Assert
            Assert.IsType<FileStreamResult>(result);
        }

        [Fact]
        public async Task ExportUserAudit_ShouldIncludeTimestampInFilename()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var testStream = new MemoryStream();

            _mediatorMock.Setup(m => m.Send(
                It.IsAny<ExportUserAuditLogsQuery>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(testStream);

            // Act
            var result = await _controller.ExportUserAuditAsync(userId);

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Matches(@"\d{8}_\d{6}\.csv$", fileResult.FileDownloadName);
        }

        [Fact]
        public async Task GetAuditLogs_ShouldReturnEmptyList_WhenNoLogsFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mediatorMock.Setup(m => m.Send(
                It.IsAny<GetUserAuditLogsQuery>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AuditLogDto>());

            // Act
            var result = await _controller.GetAuditLogsAsync(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var logs = Assert.IsType<List<AuditLogDto>>(okResult.Value);
            Assert.Empty(logs);
        }

        [Fact]
        public async Task ExportUserAudit_ShouldReturnEmptyFile_WhenNoLogsFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var emptyStream = new MemoryStream();

            _mediatorMock.Setup(m => m.Send(
                It.IsAny<ExportUserAuditLogsQuery>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyStream);

            // Act
            var result = await _controller.ExportUserAuditAsync(userId);

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal(0, fileResult.FileStream.Length);
        }
    }
}