using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.RemoveCategoryProcess;

public record RemoveCategoryProcessCommand(
    Guid CategoryId,
    Guid ProcessId) : IRequest<ProductCategoryDetailDto>;
