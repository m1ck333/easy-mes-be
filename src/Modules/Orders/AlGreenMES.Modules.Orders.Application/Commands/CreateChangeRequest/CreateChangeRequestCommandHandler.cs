using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.CreateChangeRequest;

public class CreateChangeRequestCommandHandler : IRequestHandler<CreateChangeRequestCommand, ChangeRequestDto>
{
    private readonly IChangeRequestRepository _changeRequestRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public CreateChangeRequestCommandHandler(IChangeRequestRepository changeRequestRepository, IOrdersUnitOfWork unitOfWork)
    {
        _changeRequestRepository = changeRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ChangeRequestDto> Handle(CreateChangeRequestCommand request, CancellationToken cancellationToken)
    {
        var changeRequest = ChangeRequest.Create(
            request.TenantId,
            request.OrderId,
            request.RequestedByUserId,
            request.RequestType,
            request.Description);

        await _changeRequestRepository.AddAsync(changeRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChangeRequestDto(
            changeRequest.Id,
            changeRequest.OrderId,
            changeRequest.RequestedByUserId,
            changeRequest.RequestType,
            changeRequest.Description,
            changeRequest.Status,
            changeRequest.CreatedAt,
            changeRequest.HandledByUserId,
            changeRequest.HandledAt,
            changeRequest.ResponseNote);
    }
}
