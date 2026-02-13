namespace AlGreenMES.Modules.Production.Api.Requests;

public record UpdateSpecialRequestTypeRequest(
    string Name,
    string? Description,
    List<Guid>? AddsProcesses,
    List<Guid>? RemovesProcesses,
    List<Guid>? OnlyProcesses);
