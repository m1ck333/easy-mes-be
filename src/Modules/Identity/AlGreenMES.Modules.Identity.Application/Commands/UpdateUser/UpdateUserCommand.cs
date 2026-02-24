using AlGreenMES.Modules.Identity.Application.DTOs;
using AlGreenMES.Modules.Identity.Domain.Entities;
using MediatR;

namespace AlGreenMES.Modules.Identity.Application.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    string FirstName,
    string LastName,
    UserRole Role,
    bool IsActive,
    bool CanIncludeWithdrawnInAnalysis,
    Guid? ProcessId) : IRequest<UserDto>;
