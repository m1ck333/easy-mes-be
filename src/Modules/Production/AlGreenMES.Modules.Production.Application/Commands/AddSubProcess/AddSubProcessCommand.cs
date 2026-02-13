using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.AddSubProcess;

public record AddSubProcessCommand(Guid ProcessId, string Name, int SequenceOrder) : IRequest<SubProcessDto>;
