namespace AlGreenMES.Modules.Identity.Api.Requests;

public record UpdateShiftRequest(
    string Name,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsActive);
