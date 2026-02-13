using AlGreenMES.Modules.Production.Application.Commands.CreateSpecialRequestType;
using FluentValidation;

namespace AlGreenMES.Modules.Production.Application.Validators;

public class CreateSpecialRequestTypeCommandValidator : AbstractValidator<CreateSpecialRequestTypeCommand>
{
    public CreateSpecialRequestTypeCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
