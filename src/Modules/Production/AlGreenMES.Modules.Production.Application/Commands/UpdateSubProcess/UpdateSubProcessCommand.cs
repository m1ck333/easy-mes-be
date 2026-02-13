using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.UpdateSubProcess;

public record UpdateSubProcessCommand(Guid ProcessId, Guid SubProcessId, string Name, int SequenceOrder) : IRequest<SubProcessDto>;
