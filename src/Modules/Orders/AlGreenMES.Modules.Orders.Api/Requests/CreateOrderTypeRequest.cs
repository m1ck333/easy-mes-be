namespace AlGreenMES.Modules.Orders.Api.Requests;

public class CreateOrderTypeRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool AllowsManualProcesses { get; set; }
}
