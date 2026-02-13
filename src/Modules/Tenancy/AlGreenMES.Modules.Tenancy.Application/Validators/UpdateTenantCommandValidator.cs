using AlGreenMES.Modules.Tenancy.Application.Commands.UpdateTenant;
using FluentValidation;

namespace AlGreenMES.Modules.Tenancy.Application.Validators;

public class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
