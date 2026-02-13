using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.CreateProcess;

public record CreateProcessCommand(
    Guid TenantId,
    string Code,
    string Name,
    int SequenceOrder,
    Guid? CreatedByUserId) : IRequest<ProcessDto>;
