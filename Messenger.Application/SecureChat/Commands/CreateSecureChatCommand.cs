using FluentValidation;
using MediatR;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Domain.Entities;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces.Services;

namespace Messenger.Application.SecureChat.Commands; 

// 1. CreateSecureChatCommand
public class CreateSecureChatCommand : IRequest<(string AccessKey, byte[] Salt, byte[] CreatorPublicKey)>
{
    public string Name { get; set; }
    public Guid CreatorId { get; set; }
    public Guid InvitedUserId { get; set; }
    public DateTime DestroyAt { get; set; }
    public byte[] CreatorPublicKey { get; set; }
}

public class CreateSecureChatCommandValidator : AbstractValidator<CreateSecureChatCommand>
{
    public CreateSecureChatCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Название чата не может быть пустым");
        RuleFor(x => x.CreatorId).NotEmpty().WithMessage("Идентификатор создателя не может быть пустым");
        RuleFor(x => x.InvitedUserId).NotEmpty().WithMessage("Идентификатор приглашенного пользователя не может быть пустым");
        RuleFor(x => x.DestroyAt).GreaterThan(DateTime.UtcNow).WithMessage("Время уничтожения чата должно быть в будущем");
        RuleFor(x => x.CreatorPublicKey).NotNull().WithMessage("Публичный ключ создателя не может быть нулевым");
    }
}

public class CreateSecureChatCommandHandler : IRequestHandler<CreateSecureChatCommand, (string AccessKey, byte[] Salt, byte[] CreatorPublicKey)>
{
    private readonly ISecureChatRepository _chatRepository;
    private readonly ISecureChatParticipantRepository _participantRepository;

    public CreateSecureChatCommandHandler(
        ISecureChatRepository chatRepository,
        ISecureChatParticipantRepository participantRepository)
    {
        _chatRepository = chatRepository;
        _participantRepository = participantRepository;
    }

    public async Task<(string AccessKey, byte[] Salt, byte[] CreatorPublicKey)> Handle(CreateSecureChatCommand request, CancellationToken cancellationToken)
    {
        var accessKey = Guid.NewGuid().ToString();
        var salt = new byte[16];
        RandomNumberGenerator.Fill(salt);

        var chat = new Domain.Entities.SecureChat()
        {
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            DestroyAt = request.DestroyAt,
            AccessKey = accessKey,
            Salt = salt,
            CreatorId = request.CreatorId
        };

        await _chatRepository.CreateAsync(chat);

        var creatorParticipant = new SecureChatParticipant
        {
            SecureChatId = chat.Id,
            UserId = request.CreatorId,
            PublicKey = request.CreatorPublicKey
        };
        await _participantRepository.AddAsync(creatorParticipant);

        var invitedParticipant = new SecureChatParticipant
        {
            SecureChatId = chat.Id,
            UserId = request.InvitedUserId,
            PublicKey = null
        };
        await _participantRepository.AddAsync(invitedParticipant);

        return (accessKey, salt, request.CreatorPublicKey);
    }
}

// 2. EnterSecureChatCommand (ранее JoinSecureChatCommand)
public class EnterSecureChatCommand : IRequest<(Guid ChatId, byte[] Salt, byte[] OtherPublicKey)>
{
    public string AccessKey { get; set; }
    public Guid UserId { get; set; }
    public byte[] PublicKey { get; set; }
}

public class EnterSecureChatCommandValidator : AbstractValidator<EnterSecureChatCommand>
{
    public EnterSecureChatCommandValidator()
    {
        RuleFor(x => x.AccessKey).NotEmpty().WithMessage("Ключ доступа не может быть пустым");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Идентификатор пользователя не может быть пустым");
        RuleFor(x => x.PublicKey).NotNull().WithMessage("Публичный ключ не может быть нулевым");
    }
}

public class EnterSecureChatCommandHandler : IRequestHandler<EnterSecureChatCommand, (Guid ChatId, byte[] Salt, byte[] OtherPublicKey)>
{
    private readonly ISecureChatRepository _chatRepository;
    private readonly ISecureChatParticipantRepository _participantRepository;

    public EnterSecureChatCommandHandler(
        ISecureChatRepository chatRepository,
        ISecureChatParticipantRepository participantRepository)
    {
        _chatRepository = chatRepository;
        _participantRepository = participantRepository;
    }

    public async Task<(Guid ChatId, byte[] Salt, byte[] OtherPublicKey)> Handle(EnterSecureChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await _chatRepository.GetByAccessKeyAsync(request.AccessKey);
        if (chat == null)
            throw new NotFoundException("SecureChat", request.AccessKey);

        if (chat.DestroyAt <= DateTime.UtcNow)
            throw new BusinessRuleException("Срок действия чата истек");

        var participant = await _participantRepository.GetByChatAndUserAsync(chat.Id, request.UserId);
        if (participant == null)
            throw new AccessDeniedException("присоединение к чату", chat.Id, request.UserId);

        participant.PublicKey = request.PublicKey;
        await _participantRepository.UpdateAsync(participant);

        var participants = await _participantRepository.GetParticipantsAsync(chat.Id);
        var otherParticipant = participants.FirstOrDefault(p => p.UserId != request.UserId);
        if (otherParticipant == null)
            throw new NotFoundException("SecureChatParticipant", "Второй собеседник не найден");

        return (chat.Id, chat.Salt, otherParticipant.PublicKey);
    }
}

// 3. SendEncryptedMessageCommand
public class SendEncryptedMessageCommand : IRequest<Unit>
{
    public Guid SecureChatId { get; set; }
    public Guid SenderId { get; set; }
    public byte[] Ciphertext { get; set; }
    public byte[] IV { get; set; }
    public byte[] Tag { get; set; }
}

public class SendEncryptedMessageCommandValidator : AbstractValidator<SendEncryptedMessageCommand>
{
    public SendEncryptedMessageCommandValidator()
    {
        RuleFor(x => x.SecureChatId).NotEmpty().WithMessage("Идентификатор чата не может быть пустым");
        RuleFor(x => x.SenderId).NotEmpty().WithMessage("Идентификатор отправителя не может быть пустым");
        RuleFor(x => x.Ciphertext).NotNull().WithMessage("Зашифрованный текст не может быть нулевым");
        RuleFor(x => x.IV).NotNull().WithMessage("Вектор инициализации не может быть нулевым");
        RuleFor(x => x.Tag).NotNull().WithMessage("Тег аутентификации не может быть нулевым");
    }
}

public class SendEncryptedMessageCommandHandler : IRequestHandler<SendEncryptedMessageCommand, Unit>
{
    private readonly IEncryptedMessageService _messageService;

    public SendEncryptedMessageCommandHandler(
        IEncryptedMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task<Unit> Handle(SendEncryptedMessageCommand request, CancellationToken cancellationToken)
    {
        await _messageService.SendMessageAsync(request.SecureChatId, request.SenderId, request.Ciphertext, request.IV, request.Tag);
        return Unit.Value;
    }
}

// 4. UploadEncryptedDocumentCommand
public class UploadEncryptedDocumentCommand : IRequest<Unit>
{
    public Guid SecureChatId { get; set; }
    public Guid UploaderId { get; set; }
    public byte[] FileData { get; set; }
    public byte[] IV { get; set; }
    public byte[] Tag { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
}

public class UploadEncryptedDocumentCommandValidator : AbstractValidator<UploadEncryptedDocumentCommand>
{
    public UploadEncryptedDocumentCommandValidator()
    {
        RuleFor(x => x.SecureChatId).NotEmpty().WithMessage("Идентификатор чата не может быть пустым");
        RuleFor(x => x.UploaderId).NotEmpty().WithMessage("Идентификатор загрузившего не может быть пустым");
        RuleFor(x => x.FileData).NotNull().WithMessage("Данные файла не могут быть нулевыми");
        RuleFor(x => x.IV).NotNull().WithMessage("Вектор инициализации не может быть нулевым");
        RuleFor(x => x.Tag).NotNull().WithMessage("Тег аутентификации не может быть нулевым");
        RuleFor(x => x.FileName).NotEmpty().WithMessage("Имя файла не может быть пустым");
        RuleFor(x => x.FileType).NotEmpty().WithMessage("Тип файла не может быть пустым");
    }
}

public class UploadEncryptedDocumentCommandHandler : IRequestHandler<UploadEncryptedDocumentCommand, Unit>
{
    private readonly IEncryptedDocumentService _documentService;

    public UploadEncryptedDocumentCommandHandler(
        IEncryptedDocumentService documentService)
    {
        _documentService = documentService;
    }

    public async Task<Unit> Handle(UploadEncryptedDocumentCommand request, CancellationToken cancellationToken)
    {
        await _documentService.UploadDocumentAsync(
            request.SecureChatId, 
            request.UploaderId, 
            request.FileData,
            request.IV, 
            request.Tag, 
            request.FileName, 
            request.FileType );
        
        return Unit.Value;
    }
}

// 5. DestroySecureChatCommand
public class DestroySecureChatCommand : IRequest<Unit>
{
    public Guid SecureChatId { get; set; }
    public Guid UserId { get; set; }
}

public class DestroySecureChatCommandValidator : AbstractValidator<DestroySecureChatCommand>
{
    public DestroySecureChatCommandValidator()
    {
        RuleFor(x => x.SecureChatId).NotEmpty().WithMessage("Идентификатор чата не может быть пустым");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Идентификатор пользователя не может быть пустым");
    }
}

public class DestroySecureChatCommandHandler : IRequestHandler<DestroySecureChatCommand, Unit>
{
    private readonly ISecureChatRepository _chatRepository;

    public DestroySecureChatCommandHandler(ISecureChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<Unit> Handle(DestroySecureChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await _chatRepository.GetByIdAsync(request.SecureChatId);
        if (chat == null)
            throw new NotFoundException("SecureChat", request.SecureChatId);

        if (chat.CreatorId != request.UserId)
            throw new AccessDeniedException("уничтожение чата", request.SecureChatId, request.UserId);

        await _chatRepository.DeleteAsync(request.SecureChatId);
        return Unit.Value;
    }
}