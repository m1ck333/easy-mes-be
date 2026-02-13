using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.WithdrawProcess;

public class WithdrawProcessCommandHandler : IRequestHandler<WithdrawProcessCommand, Unit>
{
    private readonly IOrderItemProcessRepository _orderItemProcessRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public WithdrawProcessCommandHandler(IOrderItemProcessRepository orderItemProcessRepository, IOrdersUnitOfWork unitOfWork)
    {
        _orderItemProcessRepository = orderItemProcessRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(WithdrawProcessCommand request, CancellationToken cancellationToken)
    {
        var process = await _orderItemProcessRepository.GetByIdAsync(request.OrderItemProcessId, cancellationToken);
        if (process == null)
            throw new NotFoundException("OrderItemProcess", request.OrderItemProcessId);

        process.Withdraw(request.UserId, request.Reason);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
