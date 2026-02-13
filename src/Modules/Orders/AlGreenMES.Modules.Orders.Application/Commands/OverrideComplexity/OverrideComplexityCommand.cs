using AlGreenMES.Modules.Production.Domain.Enums;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.OverrideComplexity;

public record OverrideComplexityCommand(Guid OrderId, Guid OrderItemId, Guid OrderItemProcessId, ComplexityType Complexity) : IRequest<Unit>;
