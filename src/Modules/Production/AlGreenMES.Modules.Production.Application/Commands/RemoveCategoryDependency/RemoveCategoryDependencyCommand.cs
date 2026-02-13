using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.RemoveCategoryDependency;

public record RemoveCategoryDependencyCommand(
    Guid CategoryId,
    Guid DependencyId) : IRequest<ProductCategoryDetailDto>;
