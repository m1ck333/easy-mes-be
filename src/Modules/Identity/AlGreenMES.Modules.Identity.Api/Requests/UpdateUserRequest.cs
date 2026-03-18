using AlGreenMES.Modules.Identity.Domain.Entities;

namespace AlGreenMES.Modules.Identity.Api.Requests;

public record UpdateUserRequest(
    Guid TenantId,
    string FirstName,
    string LastName,
    UserRole Role,
    bool IsActive,
    bool CanIncludeWithdrawnInAnalysis,
    List<Guid>? ProcessIds);
