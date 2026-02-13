using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Queries.GetProcesses;

public class GetProcessesQueryHandler : IRequestHandler<GetProcessesQuery, IReadOnlyList<ProcessDto>>
{
    private readonly IProcessRepository _processRepository;

    public GetProcessesQueryHandler(IProcessRepository processRepository)
    {
        _processRepository = processRepository;
    }

    public async Task<IReadOnlyList<ProcessDto>> Handle(GetProcessesQuery request, CancellationToken cancellationToken)
    {
        var processes = await _processRepository.GetByTenantIdAsync(request.TenantId, cancellationToken);

        return processes.Select(p => new ProcessDto(
            p.Id, p.TenantId, p.Code, p.Name, p.SequenceOrder, p.IsActive,
            p.SubProcesses.Select(sp => new SubProcessDto(sp.Id, sp.ProcessId, sp.Name, sp.SequenceOrder, sp.IsActive)).ToList()
        )).ToList();
    }
}
