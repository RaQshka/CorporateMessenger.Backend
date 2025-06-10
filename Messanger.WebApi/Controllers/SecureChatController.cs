using Messenger.Application.SecureChat.Commands;
using Messenger.Application.SecureChat.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("api/secure-chat/[action]")]
    [ApiController]
    [Authorize]
    public class SecureChatController : BaseController
    {
        /// <summary>
        /// Создает новый безопасный чат.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSecureChatRequest request)
        {
            var command = new CreateSecureChatCommand
            {
                Name = request.Name,
                CreatorId = UserId,
                InvitedUserId = request.InvitedUserId,
                DestroyAt = request.DestroyAt,
                CreatorPublicKey = request.CreatorPublicKey
            };
            var result = await Mediator.Send(command);
            return Ok(new { AccessKey = result.AccessKey, Salt = result.Salt, CreatorPublicKey = result.CreatorPublicKey });
        }

        /// <summary>
        /// Позволяет пользователю войти в существующий чат.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Enter([FromBody] EnterSecureChatRequest request)
        {
            var command = new EnterSecureChatCommand
            {
                AccessKey = request.AccessKey,
                UserId = UserId,
                PublicKey = request.PublicKey
            };
            var result = await Mediator.Send(command);
            return Ok(new { ChatId = result.ChatId, Salt = result.Salt, OtherPublicKey = result.OtherPublicKey });
        }

        /// <summary>
        /// Отправляет зашифрованное сообщение в чат.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendEncryptedMessageRequest request)
        {
            var command = new SendEncryptedMessageCommand
            {
                SecureChatId = request.SecureChatId,
                SenderId = UserId,
                Ciphertext = request.Ciphertext,
                IV = request.IV,
                Tag = request.Tag
            };
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Загружает зашифрованный документ в чат.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UploadDocument([FromBody] UploadEncryptedDocumentRequest request)
        {
            var command = new UploadEncryptedDocumentCommand
            {
                SecureChatId = request.SecureChatId,
                UploaderId = UserId,
                FileData = request.FileData,
                IV = request.IV,
                Tag = request.Tag,
                FileName = request.FileName,
                FileType = request.FileType
            };
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Удаляет чат, если пользователь является создателем.
        /// </summary>
        [HttpDelete("{chatId}")]
        public async Task<IActionResult> Destroy(Guid chatId)
        {
            var command = new DestroySecureChatCommand
            {
                SecureChatId = chatId,
                UserId = UserId
            };
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Получает активность чата (сообщения и документы).
        /// </summary>
        [HttpGet("{chatId}")]
        public async Task<IActionResult> Activity(Guid chatId, [FromQuery] int skip = 0, [FromQuery] int take = 100, [FromQuery] DateTime? fromTimestamp = null)
        {
            var query = new GetSecureChatActivityQuery
            {
                ChatId = chatId,
                UserId = UserId,
                Skip = skip,
                Take = take,
                FromTimestamp = fromTimestamp
            };
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }

    // DTO для запросов
    public class CreateSecureChatRequest
    {
        public string Name { get; set; }
        public Guid InvitedUserId { get; set; }
        public DateTime DestroyAt { get; set; }
        public byte[] CreatorPublicKey { get; set; }
    }

    public class EnterSecureChatRequest
    {
        public string AccessKey { get; set; }
        public byte[] PublicKey { get; set; }
    }

    public class SendEncryptedMessageRequest
    {
        public Guid SecureChatId { get; set; }
        public byte[] Ciphertext { get; set; }
        public byte[] IV { get; set; }
        public byte[] Tag { get; set; }
    }

    public class UploadEncryptedDocumentRequest
    {
        public Guid SecureChatId { get; set; }
        public byte[] FileData { get; set; }
        public byte[] IV { get; set; }
        public byte[] Tag { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
    }