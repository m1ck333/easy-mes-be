using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.DeleteProductCategory;

public record DeleteProductCategoryResult(bool HardDeleted, bool Deactivated, int ReferencedOrderCount);

public record DeleteProductCategoryCommand(Guid Id, bool ForceDeactivate = false, bool ForceDelete = false) : IRequest<DeleteProductCategoryResult>;
