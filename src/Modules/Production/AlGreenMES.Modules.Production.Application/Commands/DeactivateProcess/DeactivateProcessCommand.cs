using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.DeactivateProcess;

public record DeactivateProcessCommand(Guid Id) : IRequest<Unit>;
