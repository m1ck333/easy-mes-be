namespace AlGreenMES.Modules.Identity.Api.Requests;

public record LoginRequest(string Email, string Password, string TenantCode);
