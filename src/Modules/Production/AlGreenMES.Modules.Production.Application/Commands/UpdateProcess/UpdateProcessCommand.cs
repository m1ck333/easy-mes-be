using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.UpdateProcess;

public record UpdateProcessSubProcessAdd(string Name, int SequenceOrder);

public record UpdateProcessCommand(
    Guid Id,
    string Code,
    string Name,
    int SequenceOrder,
    List<UpdateProcessSubProcessAdd>? AddSubProcesses = null,
    List<Guid>? DeactivateSubProcessIds = null) : IRequest<ProcessDto>;
