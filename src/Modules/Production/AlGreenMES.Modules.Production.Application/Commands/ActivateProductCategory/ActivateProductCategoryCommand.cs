using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.ActivateProductCategory;

public record ActivateProductCategoryCommand(Guid Id) : IRequest<Unit>;
