namespace AlGreenMES.Modules.Identity.Api.Requests;

public record CreateShiftRequest(
    Guid TenantId,
    string Name,
    TimeOnly StartTime,
    TimeOnly EndTime);
