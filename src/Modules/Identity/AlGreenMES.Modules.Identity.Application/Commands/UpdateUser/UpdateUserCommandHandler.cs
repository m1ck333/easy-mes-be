using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Identity.Application.DTOs;
using AlGreenMES.Modules.Identity.Application.Interfaces;
using AlGreenMES.Modules.Identity.Domain.Entities;
using AlGreenMES.Modules.Identity.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Identity.Application.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(IUserRepository userRepository, IIdentityUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdWithProcessesAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("User", request.Id);

        user.Update(request.FirstName, request.LastName, request.Role, request.IsActive, request.CanIncludeWithdrawnInAnalysis);

        if (request.Role == UserRole.Department && request.ProcessIds != null)
            user.AssignProcesses(request.TenantId, request.ProcessIds);
        else if (request.Role != UserRole.Department)
            user.AssignProcesses(request.TenantId, []);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Adapt<UserDto>();
    }
}
