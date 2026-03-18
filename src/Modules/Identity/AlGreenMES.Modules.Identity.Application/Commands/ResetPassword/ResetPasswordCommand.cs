using MediatR;

namespace AlGreenMES.Modules.Identity.Application.Commands.ResetPassword;

public record ResetPasswordCommand(
    Guid UserId,
    string NewPassword) : IRequest<Unit>;
