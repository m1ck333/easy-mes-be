using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.UpdateProcess;

public record UpdateProcessCommand(Guid Id, string Name, int SequenceOrder) : IRequest<ProcessDto>;
