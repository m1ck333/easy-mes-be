using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
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
            throw new NotFoundException("BlockRequest", request.Id);

        blockRequest.Reject(request.HandledByUserId, request.RejectionNote);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return blockRequest.Adapt<BlockRequestDto>();
    }
}
