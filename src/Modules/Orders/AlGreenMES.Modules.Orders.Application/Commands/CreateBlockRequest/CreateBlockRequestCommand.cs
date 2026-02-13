using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.CreateBlockRequest;

public record CreateBlockRequestCommand(
    Guid TenantId,
    Guid? OrderItemProcessId,
    Guid? OrderItemSubProcessId,
    Guid RequestedByUserId,
    string? RequestNote) : IRequest<BlockRequestDto>;
