namespace AlGreenMES.Modules.Orders.Domain.Entities;

public class OrderItemSpecialRequest
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TenantId { get; private set; }
    public Guid OrderItemId { get; private set; }
    public Guid SpecialRequestTypeId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public OrderItem OrderItem { get; private set; } = null!;

    private OrderItemSpecialRequest()
    {
    }

    internal OrderItemSpecialRequest(Guid tenantId, Guid orderItemId, Guid specialRequestTypeId)
    {
        TenantId = tenantId;
        OrderItemId = orderItemId;
        SpecialRequestTypeId = specialRequestTypeId;
    }
}
