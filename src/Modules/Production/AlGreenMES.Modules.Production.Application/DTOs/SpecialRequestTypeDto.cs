namespace AlGreenMES.Modules.Production.Application.DTOs;

public record SpecialRequestTypeDto(
    Guid Id,
    Guid TenantId,
    string Code,
    string Name,
    string? Description,
    List<Guid> AddsProcesses,
    List<Guid> RemovesProcesses,
    List<Guid> OnlyProcesses,
    bool IgnoresDependencies,
    bool IsActive);
