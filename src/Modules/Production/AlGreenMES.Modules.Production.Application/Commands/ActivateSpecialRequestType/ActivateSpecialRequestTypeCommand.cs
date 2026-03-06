using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.ActivateSpecialRequestType;

public record ActivateSpecialRequestTypeCommand(Guid Id) : IRequest<Unit>;
