namespace AlGreenMES.Modules.Production.Application.DTOs;

public record ProcessDto(
    Guid Id,
    Guid TenantId,
    string Code,
    string Name,
    int SequenceOrder,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<SubProcessDto> SubProcesses);
