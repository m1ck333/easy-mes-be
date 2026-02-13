namespace AlGreenMES.Modules.Production.Api.Requests;

public record CreateSpecialRequestTypeRequest(
    Guid TenantId,
    string Code,
    string Name,
    string? Description,
    List<Guid>? AddsProcesses,
    List<Guid>? RemovesProcesses,
    List<Guid>? OnlyProcesses);
