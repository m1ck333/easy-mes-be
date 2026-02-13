using AlGreenMES.Modules.Production.Application.Commands.CreateProcess;
using FluentValidation;

namespace AlGreenMES.Modules.Production.Application.Validators;

public class CreateProcessCommandValidator : AbstractValidator<CreateProcessCommand>
{
    public CreateProcessCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SequenceOrder).GreaterThanOrEqualTo(0);
    }
}
