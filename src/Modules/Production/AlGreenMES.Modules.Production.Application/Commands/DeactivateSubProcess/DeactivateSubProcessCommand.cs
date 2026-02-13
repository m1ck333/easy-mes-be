using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.DeactivateSubProcess;

public record DeactivateSubProcessCommand(Guid ProcessId, Guid SubProcessId) : IRequest<Unit>;
