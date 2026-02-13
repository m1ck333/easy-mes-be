using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Queries.GetProcessById;

public record GetProcessByIdQuery(Guid Id) : IRequest<ProcessDto>;
