using AlGreenMES.Modules.Identity.Application.Commands.Login;
using FluentValidation;

namespace AlGreenMES.Modules.Identity.Application.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.TenantCode).NotEmpty();
    }
}
