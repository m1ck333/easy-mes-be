using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.ResumeStation;

public record ResumeStationCommand(Guid ProcessId, Guid TenantId, Guid UserId) : IRequest;
