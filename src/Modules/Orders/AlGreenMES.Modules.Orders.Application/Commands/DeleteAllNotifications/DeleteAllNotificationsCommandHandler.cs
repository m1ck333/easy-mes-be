using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.DeleteAllNotifications;

public class DeleteAllNotificationsCommandHandler : IRequestHandler<DeleteAllNotificationsCommand, Unit>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public DeleteAllNotificationsCommandHandler(INotificationRepository notificationRepository, IOrdersUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteAllNotificationsCommand request, CancellationToken cancellationToken)
    {
        await _notificationRepository.DeleteAllByUserAsync(request.UserId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
