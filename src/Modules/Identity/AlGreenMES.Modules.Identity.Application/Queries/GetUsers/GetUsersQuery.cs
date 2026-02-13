using AlGreenMES.Modules.Identity.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Identity.Application.Queries.GetUsers;

public record GetUsersQuery(Guid TenantId) : IRequest<IReadOnlyList<UserDto>>;
