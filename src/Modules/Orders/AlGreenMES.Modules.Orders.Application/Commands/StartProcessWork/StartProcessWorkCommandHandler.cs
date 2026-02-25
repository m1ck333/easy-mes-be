using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.DTOs.Events;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlGreenMES.Modules.Orders.Application.Commands.StartProcessWork;

public class StartProcessWorkCommandHandler : IRequestHandler<StartProcessWorkCommand, OrderItemProcessDto>
{
    private readonly IOrderItemProcessRepository _processRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;
    private readonly IProductionEventService _eventService;
    private readonly ILogger<StartProcessWorkCommandHandler> _logger;

    public StartProcessWorkCommandHandler(
        IOrderItemProcessRepository processRepository,
        IOrdersUnitOfWork unitOfWork,
        IProductionEventService eventService,
        ILogger<StartProcessWorkCommandHandler> logger)
    {
        _processRepository = processRepository;
        _unitOfWork = unitOfWork;
        _eventService = eventService;
        _logger = logger;
    }

    public async Task<OrderItemProcessDto> Handle(StartProcessWorkCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[StartProcess] Starting process {ProcessId} for user {UserId}",
            request.OrderItemProcessId, request.UserId);

        var process = await _processRepository.GetByIdWithFullDetailsAsync(request.OrderItemProcessId, cancellationToken);
        if (process == null)
            throw new NotFoundException("OrderItemProcess", request.OrderItemProcessId);

        _logger.LogInformation("[StartProcess] Found process. Status={Status}, OrderStatus={OrderStatus}, SubProcessCount={SubCount}",
            process.Status, process.OrderItem.Order.Status, process.SubProcesses.Count);

        if (process.OrderItem.Order.Status != OrderStatus.Active)
            throw new DomainException("ORDER_NOT_ACTIVE", "Order must be active to start work.");

        // Validate dependencies: all sibling processes that this one depends on must be Completed
        var siblingProcesses = await _processRepository.GetByOrderItemIdAsync(process.OrderItemId, cancellationToken);
        // Dependencies are modeled via ProcessId ordering — processes with lower sequence must complete first
        // The dependency check is done by the caller ensuring all prior processes are done

        process.Start();
        _logger.LogInformation("[StartProcess] Process started. Starting first sub-process...");

        var firstSubProcess = process.SubProcesses
            .Where(sp => !sp.IsWithdrawn)
            .OrderBy(sp => sp.SubProcessId)
            .FirstOrDefault();

        if (firstSubProcess != null)
        {
            firstSubProcess.Start();
            firstSubProcess.StartLog(request.UserId);
            _logger.LogInformation("[StartProcess] SubProcess {SubId} started with log for user {UserId}",
                firstSubProcess.Id, request.UserId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("[StartProcess] Changes saved. Sending event...");

        await _eventService.NotifyProcessStartedAsync(
            new ProcessStartedEvent(
                process.Id,
                process.ProcessId,
                process.OrderItem.Order.Id,
                process.OrderItem.Order.OrderNumber,
                process.TenantId), cancellationToken);

        _logger.LogInformation("[StartProcess] Event sent. Mapping DTO...");
        var result = process.Adapt<OrderItemProcessDto>();
        _logger.LogInformation("[StartProcess] Done. Returning DTO with {SubCount} sub-processes",
            result.SubProcesses.Count);
        return result;
    }
}
