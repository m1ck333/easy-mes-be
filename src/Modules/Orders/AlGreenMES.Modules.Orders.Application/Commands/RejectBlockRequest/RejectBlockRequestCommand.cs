using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.RejectBlockRequest;

public record RejectBlockRequestCommand(Guid Id, Guid HandledByUserId, string? RejectionNote) : IRequest<BlockRequestDto>;
