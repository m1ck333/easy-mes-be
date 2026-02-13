using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetWorkSessions;

public class GetWorkSessionsQueryHandler : IRequestHandler<GetWorkSessionsQuery, IReadOnlyList<WorkSessionDto>>
{
    private readonly IWorkSessionRepository _workSessionRepository;

    public GetWorkSessionsQueryHandler(IWorkSessionRepository workSessionRepository)
    {
        _workSessionRepository = workSessionRepository;
    }

    public async Task<IReadOnlyList<WorkSessionDto>> Handle(GetWorkSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _workSessionRepository.GetByTenantAndDateAsync(request.TenantId, request.Date, cancellationToken);

        return sessions.Select(s => new WorkSessionDto(
            s.Id,
            s.ProcessId,
            s.UserId,
            s.CheckInTime,
            s.CheckOutTime,
            s.DurationMinutes,
            s.Date,
            s.IsActive)).ToList();
    }
}
