using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Options;

namespace AlGreenMES.Modules.Orders.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IProcessRepository _processRepository;
    private readonly IOrderAttachmentRepository _attachmentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IOrdersUnitOfWork _unitOfWork;
    private readonly FileStorageSettings _settings;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductCategoryRepository categoryRepository,
        IProcessRepository processRepository,
        IOrderAttachmentRepository attachmentRepository,
        IFileStorageService fileStorageService,
        IOrdersUnitOfWork unitOfWork,
        IOptions<FileStorageSettings> settings)
    {
        _orderRepository = orderRepository;
        _categoryRepository = categoryRepository;
        _processRepository = processRepository;
        _attachmentRepository = attachmentRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _settings = settings.Value;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var exists = await _orderRepository.ExistsByOrderNumberAsync(request.OrderNumber, request.TenantId, cancellationToken);
        if (exists)
            throw new DomainException("ORDER_NUMBER_EXISTS", $"An order with number '{request.OrderNumber}' already exists.");

        var order = Order.Create(
            request.TenantId,
            request.OrderNumber,
            request.DeliveryDate,
            request.Priority,
            request.OrderType,
            request.CreatedByUserId,
            request.Notes);

        if (request.CustomWarningDays.HasValue || request.CustomCriticalDays.HasValue)
            order.SetCustomWarningDays(request.CustomWarningDays, request.CustomCriticalDays);

        // Add items if provided
        if (request.Items is { Count: > 0 })
        {
            foreach (var itemInput in request.Items)
            {
                var category = await _categoryRepository.GetByIdWithDetailsAsync(itemInput.ProductCategoryId, cancellationToken)
                    ?? throw new NotFoundException("ProductCategory", itemInput.ProductCategoryId);

                var item = order.AddItem(itemInput.ProductCategoryId, itemInput.ProductName, itemInput.Quantity, itemInput.Notes);

                foreach (var catProcess in category.Processes.OrderBy(p => p.SequenceOrder))
                {
                    var oip = item.AddProcess(catProcess.ProcessId, catProcess.DefaultComplexity);

                    var process = await _processRepository.GetByIdWithSubProcessesAsync(catProcess.ProcessId, cancellationToken);
                    if (process?.SubProcesses != null)
                    {
                        foreach (var sub in process.SubProcesses.OrderBy(s => s.SequenceOrder))
                        {
                            oip.AddSubProcess(sub.Id);
                        }
                    }
                }

                _orderRepository.AddItem(item);
            }
        }

        await _orderRepository.AddAsync(order, cancellationToken);

        // Process attachments if provided
        if (request.Attachments is { Count: > 0 })
        {
            foreach (var file in request.Attachments)
            {
                var contentType = (file.ContentType ?? "").ToLowerInvariant();
                if (!_settings.AllowedContentTypes.Contains(contentType))
                    throw new DomainException("INVALID_CONTENT_TYPE", $"Content type '{file.ContentType}' is not allowed.");

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension))
                {
                    extension = contentType switch
                    {
                        "image/jpeg" => ".jpg",
                        "image/png" => ".png",
                        "application/pdf" => ".pdf",
                        _ => ""
                    };
                }
                if (!_settings.AllowedExtensions.Contains(extension))
                    throw new DomainException("INVALID_FILE_TYPE", $"File type '{extension}' is not allowed. Allowed: {string.Join(", ", _settings.AllowedExtensions)}");

                if (file.FileSizeBytes > _settings.MaxFileSizeBytes)
                    throw new DomainException("FILE_TOO_LARGE", $"File size exceeds maximum of {_settings.MaxFileSizeBytes / (1024 * 1024)}MB.");

                var storedFileName = $"{Guid.NewGuid()}{extension}";
                var relativePath = Path.Combine("orders", request.TenantId.ToString(), order.Id.ToString(), storedFileName);

                await _fileStorageService.SaveFileAsync(relativePath, file.FileStream, cancellationToken);

                var attachment = OrderAttachment.Create(
                    request.TenantId,
                    order.Id,
                    file.FileName,
                    storedFileName,
                    file.ContentType,
                    file.FileSizeBytes,
                    relativePath,
                    request.CreatedByUserId);

                await _attachmentRepository.AddAsync(attachment, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Adapt<OrderDto>();
    }
}
