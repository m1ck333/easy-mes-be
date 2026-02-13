using AlGreenMES.Modules.Identity.Application.DTOs;
using AlGreenMES.Modules.Identity.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Identity.Application.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetByTenantIdAsync(request.TenantId, cancellationToken);

        return users.Select(u => new UserDto(
            u.Id,
            u.TenantId,
            u.Email,
            u.FirstName,
            u.LastName,
            u.FullName,
            u.Role,
            u.ProcessId,
            u.CanIncludeWithdrawnInAnalysis,
            u.IsActive,
            u.CreatedAt,
            u.UpdatedAt)).ToList();
    }
}
