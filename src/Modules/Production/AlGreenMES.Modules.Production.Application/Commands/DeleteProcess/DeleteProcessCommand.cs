using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.DeleteProcess;

public record DeleteProcessResult(bool HardDeleted, bool Deactivated, int ReferencedOrderCount);

public record DeleteProcessCommand(Guid Id, bool ForceDeactivate = false, bool ForceDelete = false) : IRequest<DeleteProcessResult>;
