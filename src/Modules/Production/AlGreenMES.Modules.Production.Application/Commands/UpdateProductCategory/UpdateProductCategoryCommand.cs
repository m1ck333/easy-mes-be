using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.UpdateProductCategory;

public record UpdateProductCategoryCommand(Guid Id, string Name, string? Description) : IRequest<ProductCategoryDto>;
