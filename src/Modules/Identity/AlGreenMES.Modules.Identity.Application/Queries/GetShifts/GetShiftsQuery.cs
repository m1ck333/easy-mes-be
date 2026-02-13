using AlGreenMES.Modules.Identity.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Identity.Application.Queries.GetShifts;

public record GetShiftsQuery(Guid TenantId) : IRequest<IReadOnlyList<ShiftDto>>;
