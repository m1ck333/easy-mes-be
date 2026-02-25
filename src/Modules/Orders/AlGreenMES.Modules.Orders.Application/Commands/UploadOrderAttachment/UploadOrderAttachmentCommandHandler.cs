using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Options;

namespace AlGreenMES.Modules.Orders.Application.Commands.UploadOrderAttachment;

public class UploadOrderAttachmentCommandHandler : IRequestHandler<UploadOrderAttachmentCommand, OrderAttachmentDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderAttachmentRepository _attachmentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IOrdersUnitOfWork _unitOfWork;
    private readonly FileStorageSettings _settings;

    public UploadOrderAttachmentCommandHandler(
        IOrderRepository orderRepository,
        IOrderAttachmentRepository attachmentRepository,
        IFileStorageService fileStorageService,
        IOrdersUnitOfWork unitOfWork,
        IOptions<FileStorageSettings> settings)
    {
        _orderRepository = orderRepository;
        _attachmentRepository = attachmentRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _settings = settings.Value;
    }

    public async Task<OrderAttachmentDto> Handle(UploadOrderAttachmentCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
            throw new NotFoundException("Order", request.OrderId);

        if (order.TenantId != request.TenantId)
            throw new DomainException("FORBIDDEN", "Order does not belong to this tenant.");

        // Validate file extension
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        if (!_settings.AllowedExtensions.Contains(extension))
            throw new DomainException("INVALID_FILE_TYPE", $"File type '{extension}' is not allowed. Allowed: {string.Join(", ", _settings.AllowedExtensions)}");

        // Validate content type
        if (!_settings.AllowedContentTypes.Contains(request.ContentType.ToLowerInvariant()))
            throw new DomainException("INVALID_CONTENT_TYPE", $"Content type '{request.ContentType}' is not allowed.");

        // Validate file size
        if (request.FileSizeBytes > _settings.MaxFileSizeBytes)
            throw new DomainException("FILE_TOO_LARGE", $"File size exceeds maximum of {_settings.MaxFileSizeBytes / (1024 * 1024)}MB.");

        // Validate count
        var currentCount = await _attachmentRepository.GetCountByOrderIdAsync(request.OrderId, cancellationToken);
        if (currentCount >= _settings.MaxFilesPerOrder)
            throw new DomainException("TOO_MANY_ATTACHMENTS", $"Maximum of {_settings.MaxFilesPerOrder} attachments per order.");

        // Save file
        var storedFileName = $"{Guid.NewGuid()}{extension}";
        var relativePath = Path.Combine("orders", request.TenantId.ToString(), request.OrderId.ToString(), storedFileName);

        await _fileStorageService.SaveFileAsync(relativePath, request.FileStream, cancellationToken);

        // Create entity
        var attachment = OrderAttachment.Create(
            request.TenantId,
            request.OrderId,
            request.FileName,
            storedFileName,
            request.ContentType,
            request.FileSizeBytes,
            relativePath,
            request.UserId);

        await _attachmentRepository.AddAsync(attachment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return attachment.Adapt<OrderAttachmentDto>();
    }
}
