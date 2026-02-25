using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.DeleteOrderAttachment;

public class DeleteOrderAttachmentCommandHandler : IRequestHandler<DeleteOrderAttachmentCommand, Unit>
{
    private readonly IOrderAttachmentRepository _attachmentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public DeleteOrderAttachmentCommandHandler(
        IOrderAttachmentRepository attachmentRepository,
        IFileStorageService fileStorageService,
        IOrdersUnitOfWork unitOfWork)
    {
        _attachmentRepository = attachmentRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteOrderAttachmentCommand request, CancellationToken cancellationToken)
    {
        var attachment = await _attachmentRepository.GetByIdAsync(request.AttachmentId, cancellationToken);
        if (attachment == null)
            throw new NotFoundException("OrderAttachment", request.AttachmentId);

        if (attachment.OrderId != request.OrderId)
            throw new DomainException("ATTACHMENT_NOT_FOUND", "Attachment does not belong to this order.");

        if (attachment.TenantId != request.TenantId)
            throw new DomainException("FORBIDDEN", "Attachment does not belong to this tenant.");

        await _fileStorageService.DeleteFileAsync(attachment.StoragePath, cancellationToken);
        _attachmentRepository.Remove(attachment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
