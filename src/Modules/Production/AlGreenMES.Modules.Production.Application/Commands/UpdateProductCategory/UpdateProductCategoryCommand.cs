using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Application.Commands.CreateProductCategory;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.UpdateProductCategory;

public record UpdateProductCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    List<ProcessInput>? Processes,
    List<DependencyInput>? Dependencies) : IRequest<ProductCategoryDetailDto>;
