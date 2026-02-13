using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.DeactivateSpecialRequestType;

public record DeactivateSpecialRequestTypeCommand(Guid Id) : IRequest<Unit>;
