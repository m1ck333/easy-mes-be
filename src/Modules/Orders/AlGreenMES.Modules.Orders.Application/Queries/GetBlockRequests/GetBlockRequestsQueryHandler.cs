using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetBlockRequests;

public class GetBlockRequestsQueryHandler : IRequestHandler<GetBlockRequestsQuery, PagedResult<BlockRequestDto>>
{
    private readonly IBlockRequestRepository _blockRequestRepository;

    public GetBlockRequestsQueryHandler(IBlockRequestRepository blockRequestRepository)
    {
        _blockRequestRepository = blockRequestRepository;
    }

    public async Task<PagedResult<BlockRequestDto>> Handle(GetBlockRequestsQuery request, CancellationToken cancellationToken)
    {
        var result = await _blockRequestRepository.GetPagedAsync(
            request.TenantId, request.Status, request.Search,
            request.GetCreatedFromUtc(), request.GetCreatedToUtc(),
            request.GetPage(), request.GetPageSize(), cancellationToken);

        return result.MapItems(b =>
        {
            var order = b.OrderItemProcess?.OrderItem?.Order;
            var dto = b.Adapt<BlockRequestDto>();
            return dto with { OrderId = order?.Id, OrderNumber = order?.OrderNumber, CurrentProcessStatus = b.OrderItemProcess?.Status };
        });
    }
}
