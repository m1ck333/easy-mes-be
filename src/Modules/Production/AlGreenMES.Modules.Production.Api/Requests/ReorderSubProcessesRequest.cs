namespace AlGreenMES.Modules.Production.Api.Requests;

public record ReorderSubProcessesItem(Guid Id, int SequenceOrder);

public record ReorderSubProcessesRequest(List<ReorderSubProcessesItem> Items);
