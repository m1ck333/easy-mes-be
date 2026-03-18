using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.CheckIn;

public record CheckInCommand(Guid TenantId, Guid UserId) : IRequest<WorkSessionDto>;
