using AlGreenMES.Modules.Identity.Application.Commands.UpdateUser;
using AlGreenMES.Modules.Identity.Domain.Entities;
using FluentValidation;

namespace AlGreenMES.Modules.Identity.Application.Validators;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Role).IsInEnum();
        RuleFor(x => x.ProcessIds).NotEmpty().When(x => x.Role == UserRole.Department);
    }
}
