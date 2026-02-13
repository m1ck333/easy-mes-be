using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.DTOs.Events;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.CheckOut;

public class CheckOutCommandHandler : IRequestHandler<CheckOutCommand, WorkSessionDto>
{
    private readonly IWorkSessionRepository _workSessionRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;
    private readonly IProductionEventService _eventService;

    public CheckOutCommandHandler(IWorkSessionRepository workSessionRepository, IOrdersUnitOfWork unitOfWork, IProductionEventService eventService)
    {
        _workSessionRepository = workSessionRepository;
        _unitOfWork = unitOfWork;
        _eventService = eventService;
    }

    public async Task<WorkSessionDto> Handle(CheckOutCommand request, CancellationToken cancellationToken)
    {
        var session = await _workSessionRepository.GetActiveSessionAsync(request.UserId, cancellationToken);
        if (session == null)
            throw new DomainException("NOT_CHECKED_IN", "User does not have an active session.");

        session.CheckOut();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _eventService.NotifyWorkerCheckedOutAsync(
            new WorkerCheckedOutEvent(session.UserId, session.ProcessId, session.Id, session.DurationMinutes, session.TenantId), cancellationToken);

        return new WorkSessionDto(
            session.Id,
            session.ProcessId,
            session.UserId,
            session.CheckInTime,
            session.CheckOutTime,
            session.DurationMinutes,
            session.Date,
            session.IsActive);
    }
}
