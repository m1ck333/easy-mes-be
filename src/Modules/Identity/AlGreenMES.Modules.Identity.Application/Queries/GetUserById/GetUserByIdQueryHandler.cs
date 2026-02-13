using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Identity.Application.DTOs;
using AlGreenMES.Modules.Identity.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Identity.Application.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("User", request.Id);

        return user.Adapt<UserDto>();
    }
}
