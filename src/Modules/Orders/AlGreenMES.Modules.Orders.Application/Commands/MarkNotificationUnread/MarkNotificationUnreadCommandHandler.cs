using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.MarkNotificationUnread;

public class MarkNotificationUnreadCommandHandler : IRequestHandler<MarkNotificationUnreadCommand, Unit>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public MarkNotificationUnreadCommandHandler(INotificationRepository notificationRepository, IOrdersUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(MarkNotificationUnreadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (notification == null)
            throw new NotFoundException("Notification", request.Id);

        notification.MarkUnread();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
