using AlGreenMES.Modules.Identity.Domain.Entities;

namespace AlGreenMES.Modules.Identity.Application.DTOs;

public record UserDto(
    Guid Id,
    Guid TenantId,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    UserRole Role,
    bool CanIncludeWithdrawnInAnalysis,
    bool IsActive,
    List<UserProcessDto> Processes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record UserProcessDto(
    Guid ProcessId);
