using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Messenger.Application.Common.Mappings;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Application.Interfaces.Services;
using Messenger.Application.Messages.Queries.Shared;
using Messenger.Domain.Entities;

namespace Messenger.Application.SecureChat.Queries;

// Запрос для получения активности безопасного чата
public class GetSecureChatActivityQuery : IRequest<IReadOnlyList<SecureChatActivityDto>>
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
    public DateTime? FromTimestamp { get; set; }
}

// DTO для активности чата
public class SecureChatActivityDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } // "Message" или "Document"
    public DateTime Timestamp { get; set; }
    public object Data { get; set; } // EncryptedMessageDto или EncryptedDocumentDto
}

// DTO для зашифрованного сообщения
public class EncryptedMessageDto:IMapWith<EncryptedMessage>
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public byte[] Ciphertext { get; set; }
    public byte[] IV { get; set; }
    public byte[] Tag { get; set; }
    public DateTime SentTimestamp { get; set; }
    public SenderInfoDto Sender { get; set; } = new SenderInfoDto(); 
    public void Mapping(Profile profile)
    {
        profile.CreateMap<EncryptedMessage, EncryptedMessageDto>()
            .ForMember(d => d.Id, o =>
                o.MapFrom(s => s.Id))
            .ForMember(d => d.Ciphertext, o =>
                o.MapFrom(s => s.Content))
            .ForMember(d => d.Tag, o =>
                o.MapFrom(s => s.Tag))
            .ForMember(d => d.IV, o =>
                o.MapFrom(s => s.IV))
            .ForMember(d => d.SentTimestamp, o =>
                o.MapFrom(s => s.Timestamp))
            .ForMember(d => d.Sender, o =>
                o.MapFrom(s => new SenderInfoDto
                {
                    Id = s.SenderId,
                    FirstName = s.Sender.FirstName,
                    LastName = s.Sender.LastName
                }));
        
    }

}

// DTO для зашифрованного документа
public class EncryptedDocumentDto:IMapWith<EncryptedDocument>
{
    public Guid Id { get; set; }
    public Guid UploaderId { get; set; }
    public byte[] FileData { get; set; }
    public byte[] IV { get; set; }
    public byte[] Tag { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public long FileSize { get; set; }
    public DateTime Timestamp { get; set; }
    public SenderInfoDto Sender { get; set; } = new SenderInfoDto(); 
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<EncryptedDocument, EncryptedDocumentDto>()
            .ForMember(d => d.Id, o =>
                o.MapFrom(s => s.Id))            
            .ForMember(d => d.UploaderId, o =>
                o.MapFrom(s => s.UploaderId))
            .ForMember(d => d.FileData, o =>
                o.MapFrom(s => s.FileData))
            .ForMember(d => d.Tag, o =>
                o.MapFrom(s => s.Tag))
            .ForMember(d => d.IV, o =>
                o.MapFrom(s => s.IV))
            .ForMember(d => d.Timestamp, o =>
                o.MapFrom(s => s.Timestamp))
            .ForMember(d => d.FileName, o =>
                o.MapFrom(s => s.FileName))
            .ForMember(d => d.FileSize, o =>
                o.MapFrom(s => s.FileSize))
            .ForMember(d => d.FileType, o =>
                o.MapFrom(s => s.FileType))
            .ForMember(d => d.Sender, o =>
                o.MapFrom(s => new SenderInfoDto
                {
                    Id = s.UploaderId,
                    FirstName = s.Uploader.FirstName,
                    LastName = s.Uploader.LastName
                }));
        
    }
}


// Обработчик запроса
public class
    GetSecureChatActivityQueryHandler : IRequestHandler<GetSecureChatActivityQuery,
    IReadOnlyList<SecureChatActivityDto>>
{
    private readonly IEncryptedMessageService _messageService;
    private readonly IEncryptedDocumentService _documentService;

    public GetSecureChatActivityQueryHandler( IEncryptedMessageService messageService, IEncryptedDocumentService documentService )
    {
        _messageService = messageService;
        _documentService = documentService;
    }

    public async Task<IReadOnlyList<SecureChatActivityDto>> Handle(GetSecureChatActivityQuery request,
        CancellationToken cancellationToken)
    {
        // Получение зашифрованных сообщений и документов
        var messages =
            await _messageService.GetMessagesAsync(request.ChatId, request.UserId, request.Skip, request.Take
                );
        var documents =
            await _documentService.GetDocumentsAsync(request.ChatId, request.UserId ,request.Skip, request.Take, messages.Last().SentTimestamp
                );

        // Объединение активности
        var activity = messages.Select(m => new SecureChatActivityDto
            {
                Id = m.Id,
                Type = "Message",
                Timestamp = m.SentTimestamp,
                Data = m
            }).Concat(documents.Select(d => new SecureChatActivityDto
            {
                Id = d.Id,
                Type = "Document",
                Timestamp = d.Timestamp,
                Data = d
            })).OrderByDescending(a => a.Timestamp)
            .Skip(request.Skip)
            .Take(request.Take)
            .ToList();

        return activity;
    }
}

