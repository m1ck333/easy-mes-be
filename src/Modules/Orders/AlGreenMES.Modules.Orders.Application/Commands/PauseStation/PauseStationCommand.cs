using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.PauseStation;

public record PauseStationCommand(Guid ProcessId, Guid TenantId, Guid UserId) : IRequest;
