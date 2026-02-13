namespace AlGreenMES.Modules.Identity.Api.Requests;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
