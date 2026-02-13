using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetBlockRequests;

public class GetBlockRequestsQueryHandler : IRequestHandler<GetBlockRequestsQuery, IReadOnlyList<BlockRequestDto>>
{
    private readonly IBlockRequestRepository _blockRequestRepository;

    public GetBlockRequestsQueryHandler(IBlockRequestRepository blockRequestRepository)
    {
        _blockRequestRepository = blockRequestRepository;
    }

    public async Task<IReadOnlyList<BlockRequestDto>> Handle(GetBlockRequestsQuery request, CancellationToken cancellationToken)
    {
        var blockRequests = await _blockRequestRepository.GetByTenantIdAsync(request.TenantId, request.Status, cancellationToken);

        return blockRequests.Select(b => new BlockRequestDto(
            b.Id,
            b.OrderItemProcessId,
            b.OrderItemSubProcessId,
            b.RequestedByUserId,
            b.RequestNote,
            b.Status,
            b.CreatedAt,
            b.HandledByUserId,
            b.HandledAt,
            b.BlockReason,
            b.RejectionNote)).ToList();
    }
}
