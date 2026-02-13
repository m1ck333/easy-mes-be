using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.RejectChangeRequest;

public record RejectChangeRequestCommand(Guid Id, Guid HandledByUserId, string? ResponseNote) : IRequest<ChangeRequestDto>;
