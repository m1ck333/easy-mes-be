using AlGreenMES.Modules.Tenancy.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Tenancy.Application.Queries.GetTenantById;

public record GetTenantByIdQuery(Guid Id) : IRequest<TenantDto?>;
