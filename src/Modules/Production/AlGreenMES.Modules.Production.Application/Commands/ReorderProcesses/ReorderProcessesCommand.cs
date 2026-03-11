using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.ReorderProcesses;

public record ReorderProcessesItem(Guid Id, int SequenceOrder);

public record ReorderProcessesCommand(List<ReorderProcessesItem> Items) : IRequest<Unit>;
