namespace AlGreenMES.Modules.Production.Application.DTOs;

public record ProcessDto(
    Guid Id,
    Guid TenantId,
    string Code,
    string Name,
    int SequenceOrder,
    bool IsActive,
    List<SubProcessDto> SubProcesses);
