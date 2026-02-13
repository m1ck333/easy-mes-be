using AlGreenMES.Modules.Tenancy.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Tenancy.Application.Commands.UpdateTenant;

public record UpdateTenantCommand(Guid Id, string Name, bool IsActive) : IRequest<TenantDto>;
