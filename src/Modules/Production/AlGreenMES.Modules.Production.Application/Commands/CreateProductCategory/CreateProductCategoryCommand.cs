using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.CreateProductCategory;

public record CreateProductCategoryCommand(
    Guid TenantId,
    string Name,
    string? Description,
    Guid? CreatedByUserId) : IRequest<ProductCategoryDto>;
