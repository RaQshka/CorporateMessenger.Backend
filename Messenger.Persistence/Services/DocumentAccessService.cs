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
    private readonly UserManager<User> _userManager;
    private readonly IChatAccessService _chatAccessService;

    public DocumentAccessService(
        IDocumentAccessRepository documentAccessRepository,
        IDocumentRepository documentRepository,
        UserManager<User> userManager,
        IChatAccessService chatAccessService)
    {
        _documentAccessRepository = documentAccessRepository;
        _documentRepository = documentRepository;
        _userManager = userManager;
        _chatAccessService = chatAccessService;
    }

    public async Task GrantAccessAsync(Guid documentId, Guid roleId, DocumentAccess accessFlag, CancellationToken ct)
    {
        var document = await _documentRepository.GetByIdAsync(documentId, ct)
                       ?? throw new NotFoundException("Документ", documentId);

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

    public async Task RevokeAccessAsync(Guid documentId, Guid roleId, DocumentAccess accessFlag, CancellationToken ct)
    {
        var document = await _documentRepository.GetByIdAsync(documentId, ct)
                       ?? throw new NotFoundException("Документ", documentId);

        var rule = await _documentAccessRepository.GetRuleAsync(documentId, roleId, ct)
                   ?? throw new NotFoundException("Правило доступа", $"DocumentId: {documentId}, RoleId: {roleId}");

        rule.DocumentAccessMask &= ~(int)accessFlag;
        if (rule.DocumentAccessMask == 0)
            await _documentAccessRepository.RemoveRuleAsync(documentId, roleId, ct);
        else
            await _documentAccessRepository.UpdateRuleAsync(rule, ct);
    }

    public async Task<bool> HasAccessAsync(Guid documentId, Guid userId, DocumentAccess accessFlag, CancellationToken ct)
    {
        var document = await _documentRepository.GetByIdAsync(documentId, ct)
                       ?? throw new NotFoundException("Документ", documentId);

        if (await _chatAccessService.IsAdminOfChat(document.ChatId, userId, ct))
            return true;

        if (document.UploaderId == userId)
            return true;

        var user = await _userManager.FindByIdAsync(userId.ToString())
                   ?? throw new NotFoundException("Пользователь", userId);

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Any())
            return false;

        var rules = await _documentAccessRepository.GetRulesByDocumentAsync(documentId, ct);
        var combinedMask = rules
            .Where(r => roles.Contains(r.Role.Name))
            .Aggregate(0, (current, rule) => current | rule.DocumentAccessMask);

        return (combinedMask & (int)accessFlag) == (int)accessFlag;
    }

    public async Task<IReadOnlyList<DocumentAccessRule>> GetRulesAsync(Guid documentId, CancellationToken ct)
    {
        var document = await _documentRepository.GetByIdAsync(documentId, ct)
                       ?? throw new NotFoundException("Документ", documentId);

        return await _documentAccessRepository.GetRulesByDocumentAsync(documentId, ct);
    }
}