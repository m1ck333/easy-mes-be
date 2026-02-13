using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.RejectBlockRequest;

public class RejectBlockRequestCommandHandler : IRequestHandler<RejectBlockRequestCommand, BlockRequestDto>
{
    private readonly IBlockRequestRepository _blockRequestRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public RejectBlockRequestCommandHandler(IBlockRequestRepository blockRequestRepository, IOrdersUnitOfWork unitOfWork)
    {
        _blockRequestRepository = blockRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BlockRequestDto> Handle(RejectBlockRequestCommand request, CancellationToken cancellationToken)
    {
        var blockRequest = await _blockRequestRepository.GetByIdAsync(request.Id, cancellationToken);
        if (blockRequest == null)
            throw new DomainException("BLOCK_REQUEST_NOT_FOUND", $"Block request with id '{request.Id}' was not found.");

        blockRequest.Reject(request.HandledByUserId, request.RejectionNote);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BlockRequestDto(
            blockRequest.Id,
            blockRequest.OrderItemProcessId,
            blockRequest.OrderItemSubProcessId,
            blockRequest.RequestedByUserId,
            blockRequest.RequestNote,
            blockRequest.Status,
            blockRequest.CreatedAt,
            blockRequest.HandledByUserId,
            blockRequest.HandledAt,
            blockRequest.BlockReason,
            blockRequest.RejectionNote);
    }
}
