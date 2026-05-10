using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;
using OrderTypeEntity = AlGreenMES.Modules.Orders.Domain.Entities.OrderTypes.OrderType;

namespace AlGreenMES.Modules.Orders.Application.Commands.CreateOrderType;

public class CreateOrderTypeCommandHandler : IRequestHandler<CreateOrderTypeCommand, OrderTypeDto>
{
    private readonly IOrderTypeRepository _repository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public CreateOrderTypeCommandHandler(IOrderTypeRepository repository, IOrdersUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderTypeDto> Handle(CreateOrderTypeCommand request, CancellationToken cancellationToken)
    {
        if (await _repository.ExistsByCodeAsync(request.Code, request.TenantId, cancellationToken))
            throw new DomainException("ORDER_TYPE_CODE_EXISTS", $"An order type with code '{request.Code}' already exists.");

        var orderType = OrderTypeEntity.Create(request.TenantId, request.Code, request.Name, request.AllowsManualProcesses);
        await _repository.AddAsync(orderType, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return orderType.Adapt<OrderTypeDto>();
    }
}
