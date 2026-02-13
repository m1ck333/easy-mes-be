using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Queries.GetProductCategoryById;

public record GetProductCategoryByIdQuery(Guid Id) : IRequest<ProductCategoryDetailDto>;
