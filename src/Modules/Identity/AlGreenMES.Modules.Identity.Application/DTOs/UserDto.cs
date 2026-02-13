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
    Guid? ProcessId,
    bool CanIncludeWithdrawnInAnalysis,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
