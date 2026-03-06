using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.ActivateProcess;

public record ActivateProcessCommand(Guid Id) : IRequest<Unit>;
