namespace AlGreenMES.Modules.Production.Api.Requests;

public record ReorderProcessesItem(Guid Id, int SequenceOrder);

public record ReorderProcessesRequest(List<ReorderProcessesItem> Items);
