using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.AddCategoryDependency;

public record AddCategoryDependencyCommand(
    Guid CategoryId,
    Guid ProcessId,
    Guid DependsOnProcessId) : IRequest<ProductCategoryDetailDto>;
