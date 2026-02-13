namespace AlGreenMES.Modules.Production.Application.DTOs;

public record SubProcessDto(
    Guid Id,
    Guid ProcessId,
    string Name,
    int SequenceOrder,
    bool IsActive);
