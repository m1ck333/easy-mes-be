using AlGreenMES.Modules.Tenancy.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Tenancy.Application.Queries.GetTenantSettings;

public record GetTenantSettingsQuery(Guid TenantId) : IRequest<TenantSettingsDto>;
