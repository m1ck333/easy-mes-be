using AlGreenMES.Modules.Orders.Application.Commands.AddOrderItem;
using FluentValidation;

namespace AlGreenMES.Modules.Orders.Application.Validators;

public class AddOrderItemCommandValidator : AbstractValidator<AddOrderItemCommand>
{
    public AddOrderItemCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.ProductCategoryId).NotEmpty();
        RuleFor(x => x.ProductName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
