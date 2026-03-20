using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.ReorderSubProcesses;

public record ReorderSubProcessesItem(Guid Id, int SequenceOrder);

public record ReorderSubProcessesCommand(Guid ProcessId, List<ReorderSubProcessesItem> Items) : IRequest<Unit>;
