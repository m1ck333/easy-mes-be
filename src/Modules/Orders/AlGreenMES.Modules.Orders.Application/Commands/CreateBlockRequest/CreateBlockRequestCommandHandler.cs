using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.DTOs.Events;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.CreateBlockRequest;

public class CreateBlockRequestCommandHandler : IRequestHandler<CreateBlockRequestCommand, BlockRequestDto>
{
    private readonly IBlockRequestRepository _blockRequestRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;
    private readonly IProductionEventService _eventService;

    public CreateBlockRequestCommandHandler(IBlockRequestRepository blockRequestRepository, IOrdersUnitOfWork unitOfWork, IProductionEventService eventService)
    {
        _blockRequestRepository = blockRequestRepository;
        _unitOfWork = unitOfWork;
        _eventService = eventService;
    }

    public async Task<BlockRequestDto> Handle(CreateBlockRequestCommand request, CancellationToken cancellationToken)
    {
        if (!request.OrderItemProcessId.HasValue && !request.OrderItemSubProcessId.HasValue)
            throw new DomainException("TARGET_REQUIRED", "Either OrderItemProcessId or OrderItemSubProcessId must be specified.");

        BlockRequest blockRequest;

        if (request.OrderItemProcessId.HasValue)
        {
            blockRequest = BlockRequest.CreateForProcess(
                request.TenantId,
                request.OrderItemProcessId.Value,
                request.RequestedByUserId,
                request.RequestNote);
        }
        else
        {
            blockRequest = BlockRequest.CreateForSubProcess(
                request.TenantId,
                request.OrderItemSubProcessId!.Value,
                request.RequestedByUserId,
                request.RequestNote);
        }

        await _blockRequestRepository.AddAsync(blockRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _eventService.NotifyBlockRequestCreatedAsync(
            new BlockRequestCreatedEvent(
                blockRequest.Id,
                blockRequest.OrderItemProcessId,
                blockRequest.OrderItemSubProcessId,
                blockRequest.RequestNote,
                request.TenantId), cancellationToken);

        return blockRequest.Adapt<BlockRequestDto>();
    }
}
