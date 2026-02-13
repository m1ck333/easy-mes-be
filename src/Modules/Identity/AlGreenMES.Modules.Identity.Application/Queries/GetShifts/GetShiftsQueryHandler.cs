using AlGreenMES.Modules.Identity.Application.DTOs;
using AlGreenMES.Modules.Identity.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Identity.Application.Queries.GetShifts;

public class GetShiftsQueryHandler : IRequestHandler<GetShiftsQuery, IReadOnlyList<ShiftDto>>
{
    private readonly IShiftRepository _shiftRepository;

    public GetShiftsQueryHandler(IShiftRepository shiftRepository)
    {
        _shiftRepository = shiftRepository;
    }

    public async Task<IReadOnlyList<ShiftDto>> Handle(GetShiftsQuery request, CancellationToken cancellationToken)
    {
        var shifts = await _shiftRepository.GetByTenantIdAsync(request.TenantId, cancellationToken);

        return shifts.Select(s => new ShiftDto(
            s.Id,
            s.TenantId,
            s.Name,
            s.StartTime,
            s.EndTime,
            s.IsActive)).ToList();
    }
}
