using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Persistence.Services;

public class DocumentAccessService : IDocumentAccessService
{
    private readonly IDocumentAccessRepository _documentAccessRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IChatAccessService _chatAccessService;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public DocumentAccessService(
        IDocumentAccessRepository documentAccessRepository,
        IDocumentRepository documentRepository,
        IChatAccessService chatAccessService,
        UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
        _documentAccessRepository = documentAccessRepository;
        _documentRepository = documentRepository;
        _chatAccessService = chatAccessService;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> HasAccessAsync(Guid documentId, Guid userId, DocumentAccess accessFlag, CancellationToken ct)
    {
        var document = await _documentRepository.GetByIdAsync(documentId, ct)
                       ?? throw new NotFoundException("Документ", documentId);

        var user = await _userManager.FindByIdAsync(userId.ToString())
                   ?? throw new NotFoundException("Пользователь", userId);

        // Администратор чата или загрузчик документа имеют полный доступ
        if (document.UploaderId == userId || await _chatAccessService.IsAdminOfChat(document.ChatId, userId, ct))
            return true;

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Any())
            return false;

        int combinedMask = 0;
        foreach (var role in roles)
        {
            var rule = await _documentAccessRepository.GetRuleAsync(documentId, role, ct);
            if (rule != null)
                combinedMask |= rule.DocumentAccessMask;
        }

        return (combinedMask & (int)accessFlag) == (int)accessFlag;
    }

    public async Task GrantAccessAsync(Guid documentId, Guid initiatorId, Guid roleId, DocumentAccess accessFlag, CancellationToken ct)
    {
        var document = await _documentRepository.GetByIdAsync(documentId, ct)
                       ?? throw new NotFoundException("Документ", documentId);

        // Проверка, является ли инициатор создателем документа
        if (!await IsCreatorOfDocument(documentId, initiatorId, ct))
            throw new AccessDeniedException("Предоставление доступа к документу", document.ChatId, initiatorId);

        // Проверка существования роли
        var role = await _roleManager.FindByIdAsync(roleId.ToString())
                   ?? throw new NotFoundException("Роль", roleId);

        var rule = await _documentAccessRepository.GetRuleAsync(documentId, roleId, ct);
        if (rule == null)
        {
            rule = new DocumentAccessRule
            {
                Id = Guid.NewGuid(),
                DocumentId = documentId,
                RoleId = roleId,
                DocumentAccessMask = (int)accessFlag
            };
            await _documentAccessRepository.AddRuleAsync(rule, ct);
        }
        else
        {
            rule.DocumentAccessMask |= (int)accessFlag;
            await _documentAccessRepository.UpdateRuleAsync(rule, ct);
        }
    }

    public async Task RevokeAccessAsync(Guid documentId, Guid initiatorId, Guid roleId, DocumentAccess accessFlag, CancellationToken ct)
    {
        var document = await _documentRepository.GetByIdAsync(documentId, ct)
                       ?? throw new NotFoundException("Документ", documentId);

        // Проверка, является ли инициатор создателем документа
        if (!await IsCreatorOfDocument(documentId, initiatorId, ct))
            throw new AccessDeniedException("Отзыв доступа к документу", document.ChatId, initiatorId);

        // Проверка существования роли
        var role = await _roleManager.FindByIdAsync(roleId.ToString())
                   ?? throw new NotFoundException("Роль", roleId);

        var rule = await _documentAccessRepository.GetRuleAsync(documentId, roleId, ct)
                   ?? throw new NotFoundException("Правило доступа", $"DocumentId: {documentId}, RoleId: {roleId}");

        rule.DocumentAccessMask &= ~(int)accessFlag;
        if (rule.DocumentAccessMask == 0)
            await _documentAccessRepository.RemoveRuleAsync(documentId, roleId, ct);
        else
            await _documentAccessRepository.UpdateRuleAsync(rule, ct);
    }

    public async Task<IReadOnlyList<DocumentAccessRule>> GetRulesAsync(Guid documentId, CancellationToken ct)
    {
        var document = await _documentRepository.GetByIdAsync(documentId, ct)
                       ?? throw new NotFoundException("Документ", documentId);

        return await _documentAccessRepository.GetRulesByDocumentAsync(documentId, ct);
    }
    private async Task<bool> IsCreatorOfDocument(Guid documentId, Guid initiatorId, CancellationToken ct)
    {
        var document = await _documentRepository.GetByIdAsync(documentId, ct)
                       ?? throw new NotFoundException("Документ", documentId);
        return document.UploaderId == initiatorId;
    }
}