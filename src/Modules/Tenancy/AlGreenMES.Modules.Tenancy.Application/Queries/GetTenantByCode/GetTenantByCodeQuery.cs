using AlGreenMES.Modules.Tenancy.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Tenancy.Application.Queries.GetTenantByCode;

public record GetTenantByCodeQuery(string Code) : IRequest<TenantDto?>;
