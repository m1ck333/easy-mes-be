using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.DTOs.Events;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.ApproveBlockRequest;

public class ApproveBlockRequestCommandHandler : IRequestHandler<ApproveBlockRequestCommand, BlockRequestDto>
{
    private readonly IBlockRequestRepository _blockRequestRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;
    private readonly IProductionEventService _eventService;

    public ApproveBlockRequestCommandHandler(IBlockRequestRepository blockRequestRepository, IOrdersUnitOfWork unitOfWork, IProductionEventService eventService)
    {
        _blockRequestRepository = blockRequestRepository;
        _unitOfWork = unitOfWork;
        _eventService = eventService;
    }

    public async Task<BlockRequestDto> Handle(ApproveBlockRequestCommand request, CancellationToken cancellationToken)
    {
        var blockRequest = await _blockRequestRepository.GetByIdAsync(request.Id, cancellationToken);
        if (blockRequest == null)
            throw new NotFoundException("BlockRequest", request.Id);

        blockRequest.Approve(request.HandledByUserId, request.BlockReason);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _eventService.NotifyBlockRequestApprovedAsync(
            new BlockRequestApprovedEvent(
                blockRequest.Id,
                blockRequest.OrderItemProcessId,
                blockRequest.OrderItemSubProcessId,
                request.BlockReason,
                blockRequest.TenantId), cancellationToken);

        return blockRequest.Adapt<BlockRequestDto>();
    }
}
