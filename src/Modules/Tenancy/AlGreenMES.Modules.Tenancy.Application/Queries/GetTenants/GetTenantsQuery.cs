using AlGreenMES.Modules.Tenancy.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Tenancy.Application.Queries.GetTenants;

public record GetTenantsQuery : IRequest<IReadOnlyList<TenantDto>>;
