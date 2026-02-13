using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.ApproveBlockRequest;

public record ApproveBlockRequestCommand(Guid Id, Guid HandledByUserId, string BlockReason) : IRequest<BlockRequestDto>;
