using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Queries.GetProcesses;

public record GetProcessesQuery(Guid TenantId) : IRequest<IReadOnlyList<ProcessDto>>;
