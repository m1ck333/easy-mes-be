using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.ApproveChangeRequest;

public record ApproveChangeRequestCommand(Guid Id, Guid HandledByUserId, string? ResponseNote) : IRequest<ChangeRequestDto>;
