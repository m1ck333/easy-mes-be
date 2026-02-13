namespace AlGreenMES.Modules.Orders.Api.Requests;

public record HandleBlockRequestRequest(Guid HandledByUserId, string? Note);
