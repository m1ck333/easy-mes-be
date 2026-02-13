using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Domain.Enums;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.AddCategoryProcess;

public record AddCategoryProcessCommand(
    Guid CategoryId,
    Guid ProcessId,
    int SequenceOrder,
    ComplexityType? DefaultComplexity) : IRequest<ProductCategoryDetailDto>;
