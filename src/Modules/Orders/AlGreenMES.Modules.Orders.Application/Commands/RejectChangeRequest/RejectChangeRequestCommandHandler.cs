using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.RejectChangeRequest;

public class RejectChangeRequestCommandHandler : IRequestHandler<RejectChangeRequestCommand, ChangeRequestDto>
{
    private readonly IChangeRequestRepository _changeRequestRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public RejectChangeRequestCommandHandler(IChangeRequestRepository changeRequestRepository, IOrdersUnitOfWork unitOfWork)
    {
        _changeRequestRepository = changeRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ChangeRequestDto> Handle(RejectChangeRequestCommand request, CancellationToken cancellationToken)
    {
        var changeRequest = await _changeRequestRepository.GetByIdAsync(request.Id, cancellationToken);
        if (changeRequest == null)
            throw new NotFoundException("ChangeRequest", request.Id);

        changeRequest.Reject(request.HandledByUserId, request.ResponseNote);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return changeRequest.Adapt<ChangeRequestDto>();
    }
}
