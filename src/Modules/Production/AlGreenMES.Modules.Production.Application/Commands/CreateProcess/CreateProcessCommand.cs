using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.CreateProcess;

public record CreateProcessSubProcessItem(string Name, int SequenceOrder);

public record CreateProcessCommand(
    Guid TenantId,
    string Code,
    string Name,
    int SequenceOrder,
    Guid? CreatedByUserId,
    List<CreateProcessSubProcessItem>? SubProcesses = null) : IRequest<ProcessDto>;
