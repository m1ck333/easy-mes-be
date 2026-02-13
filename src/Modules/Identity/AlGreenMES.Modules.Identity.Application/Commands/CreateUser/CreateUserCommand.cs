using AlGreenMES.Modules.Identity.Application.DTOs;
using AlGreenMES.Modules.Identity.Domain.Entities;
using MediatR;

namespace AlGreenMES.Modules.Identity.Application.Commands.CreateUser;

public record CreateUserCommand(
    Guid TenantId,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    UserRole Role) : IRequest<UserDto>;
