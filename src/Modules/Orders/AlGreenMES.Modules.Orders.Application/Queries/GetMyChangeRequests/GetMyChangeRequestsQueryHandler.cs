using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetMyChangeRequests;

public class GetMyChangeRequestsQueryHandler : IRequestHandler<GetMyChangeRequestsQuery, PagedResult<ChangeRequestDto>>
{
    private readonly IChangeRequestRepository _changeRequestRepository;

    public GetMyChangeRequestsQueryHandler(IChangeRequestRepository changeRequestRepository)
    {
        _changeRequestRepository = changeRequestRepository;
    }

    public async Task<PagedResult<ChangeRequestDto>> Handle(GetMyChangeRequestsQuery request, CancellationToken cancellationToken)
    {
        var result = await _changeRequestRepository.GetPagedByUserAsync(
            request.TenantId, request.UserId, request.Status, request.Search,
            request.GetPage(), request.GetPageSize(), cancellationToken);

        return result.MapItems(c => c.Adapt<ChangeRequestDto>());
    }
}
