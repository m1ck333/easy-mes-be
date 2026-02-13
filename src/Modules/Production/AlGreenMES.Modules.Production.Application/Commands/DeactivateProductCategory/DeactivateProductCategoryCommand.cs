using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.DeactivateProductCategory;

public record DeactivateProductCategoryCommand(Guid Id) : IRequest<Unit>;
