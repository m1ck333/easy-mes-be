using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.CreateSpecialRequestType;

public record CreateSpecialRequestTypeCommand(
    Guid TenantId,
    string Code,
    string Name,
    string? Description,
    List<Guid>? AddsProcesses,
    List<Guid>? RemovesProcesses,
    List<Guid>? OnlyProcesses) : IRequest<SpecialRequestTypeDto>;
