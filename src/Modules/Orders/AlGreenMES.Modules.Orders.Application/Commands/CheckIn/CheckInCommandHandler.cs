using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.DTOs.Events;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.CheckIn;

public class CheckInCommandHandler : IRequestHandler<CheckInCommand, WorkSessionDto>
{
    private readonly IWorkSessionRepository _workSessionRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;
    private readonly IProductionEventService _eventService;

    public CheckInCommandHandler(
        IWorkSessionRepository workSessionRepository,
        IOrdersUnitOfWork unitOfWork,
        IProductionEventService eventService)
    {
        _workSessionRepository = workSessionRepository;
        _unitOfWork = unitOfWork;
        _eventService = eventService;
    }

    public async Task<WorkSessionDto> Handle(CheckInCommand request, CancellationToken cancellationToken)
    {
        var active = await _workSessionRepository.GetActiveSessionAsync(request.UserId, cancellationToken);
        if (active != null)
            throw new DomainException("ALREADY_CHECKED_IN", "User already has an active session.");

        var session = WorkSession.CheckIn(request.TenantId, request.UserId);

        await _workSessionRepository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _eventService.NotifyWorkerCheckedInAsync(
            new WorkerCheckedInEvent(request.UserId, session.Id, request.TenantId), cancellationToken);

        return session.Adapt<WorkSessionDto>();
    }
}
