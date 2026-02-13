using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetChangeRequests;

public class GetChangeRequestsQueryHandler : IRequestHandler<GetChangeRequestsQuery, IReadOnlyList<ChangeRequestDto>>
{
    private readonly IChangeRequestRepository _changeRequestRepository;

    public GetChangeRequestsQueryHandler(IChangeRequestRepository changeRequestRepository)
    {
        _changeRequestRepository = changeRequestRepository;
    }

    public async Task<IReadOnlyList<ChangeRequestDto>> Handle(GetChangeRequestsQuery request, CancellationToken cancellationToken)
    {
        var changeRequests = await _changeRequestRepository.GetByTenantIdAsync(request.TenantId, request.Status, cancellationToken);

        return changeRequests.Select(c => new ChangeRequestDto(
            c.Id,
            c.OrderId,
            c.RequestedByUserId,
            c.RequestType,
            c.Description,
            c.Status,
            c.CreatedAt,
            c.HandledByUserId,
            c.HandledAt,
            c.ResponseNote)).ToList();
    }
}
