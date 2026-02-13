using AlGreenMES.Modules.Orders.Application.Commands.UpdateOrder;
using FluentValidation;

namespace AlGreenMES.Modules.Orders.Application.Validators;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CustomWarningDays).GreaterThan(0).When(x => x.CustomWarningDays.HasValue);
        RuleFor(x => x.CustomCriticalDays).GreaterThan(0).When(x => x.CustomCriticalDays.HasValue);
    }
}
