using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetMyChangeRequests;

public class GetMyChangeRequestsQueryHandler : IRequestHandler<GetMyChangeRequestsQuery, IReadOnlyList<ChangeRequestDto>>
{
    private readonly IChangeRequestRepository _changeRequestRepository;

    public GetMyChangeRequestsQueryHandler(IChangeRequestRepository changeRequestRepository)
    {
        _changeRequestRepository = changeRequestRepository;
    }

    public async Task<IReadOnlyList<ChangeRequestDto>> Handle(GetMyChangeRequestsQuery request, CancellationToken cancellationToken)
    {
        var changeRequests = await _changeRequestRepository.GetByUserIdAsync(request.UserId, cancellationToken);

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
