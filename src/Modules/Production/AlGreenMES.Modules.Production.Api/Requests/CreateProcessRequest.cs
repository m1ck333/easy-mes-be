namespace AlGreenMES.Modules.Production.Api.Requests;

public record CreateProcessRequest(
    Guid TenantId,
    string Code,
    string Name,
    int SequenceOrder);
