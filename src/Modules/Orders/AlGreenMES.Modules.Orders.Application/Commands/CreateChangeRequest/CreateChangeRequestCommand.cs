using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Enums;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.CreateChangeRequest;

public record CreateChangeRequestCommand(
    Guid TenantId,
    Guid OrderId,
    Guid RequestedByUserId,
    ChangeRequestType RequestType,
    string Description) : IRequest<ChangeRequestDto>;
