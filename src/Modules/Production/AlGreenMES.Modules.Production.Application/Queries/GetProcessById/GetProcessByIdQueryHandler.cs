using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Queries.GetProcessById;

public class GetProcessByIdQueryHandler : IRequestHandler<GetProcessByIdQuery, ProcessDto?>
{
    private readonly IProcessRepository _processRepository;

    public GetProcessByIdQueryHandler(IProcessRepository processRepository)
    {
        _processRepository = processRepository;
    }

    public async Task<ProcessDto?> Handle(GetProcessByIdQuery request, CancellationToken cancellationToken)
    {
        var process = await _processRepository.GetByIdWithSubProcessesAsync(request.Id, cancellationToken);
        if (process is null) return null;

        return new ProcessDto(
            process.Id, process.TenantId, process.Code, process.Name, process.SequenceOrder, process.IsActive,
            process.SubProcesses.Select(sp => new SubProcessDto(sp.Id, sp.ProcessId, sp.Name, sp.SequenceOrder, sp.IsActive)).ToList());
    }
}
