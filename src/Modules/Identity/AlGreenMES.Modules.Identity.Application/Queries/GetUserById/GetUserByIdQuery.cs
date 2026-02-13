using AlGreenMES.Modules.Identity.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Identity.Application.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;
