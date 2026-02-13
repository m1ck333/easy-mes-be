using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetWorkSessions;

public class GetWorkSessionsQueryHandler : IRequestHandler<GetWorkSessionsQuery, PagedResult<WorkSessionDto>>
{
    private readonly IWorkSessionRepository _workSessionRepository;

    public GetWorkSessionsQueryHandler(IWorkSessionRepository workSessionRepository)
    {
        _workSessionRepository = workSessionRepository;
    }

    public async Task<PagedResult<WorkSessionDto>> Handle(GetWorkSessionsQuery request, CancellationToken cancellationToken)
    {
        var result = await _workSessionRepository.GetPagedAsync(
            request.TenantId, request.Date, request.UserId,
            request.GetPage(), request.GetPageSize(), cancellationToken);

        return result.MapItems(s => s.Adapt<WorkSessionDto>());
    }
}
