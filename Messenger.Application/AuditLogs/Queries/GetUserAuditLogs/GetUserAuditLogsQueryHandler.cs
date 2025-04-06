using AutoMapper;
using MediatR;
using Messenger.Application.AuditLogs.Queries.Shared;
using Messenger.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Application.AuditLogs.Queries.GetUserAuditLogs;

public class GetUserAuditLogsQueryHandler : IRequestHandler<GetUserAuditLogsQuery, List<AuditLogDto>>
{
    private readonly IMessengerDbContext _context;
    private readonly IMapper _mapper;

    public GetUserAuditLogsQueryHandler(IMessengerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<AuditLogDto>> Handle(GetUserAuditLogsQuery request, CancellationToken cancellationToken)
    {
        if(request.UserId == null) return new List<AuditLogDto>();
        
        var endDate = request.EndDate ?? DateTime.UtcNow;
        var startDate = request.StartTime ?? endDate.AddDays(-(request.Days ?? 30));

        var logs = await _context.AuditLogs
            .Where(log => log.UserId == request.UserId)
            .Where(log => log.ActionTime >= startDate && log.ActionTime <= endDate)
            .OrderByDescending(log => log.ActionTime)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<AuditLogDto>>(logs);

    }
}