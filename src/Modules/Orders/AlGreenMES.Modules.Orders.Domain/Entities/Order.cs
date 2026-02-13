using AlGreenMES.BuildingBlocks.Common.Entities;
using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Domain.Entities;

public class Order : AuditableEntity
{
    public string OrderNumber { get; private set; } = null!;
    public DateTime DeliveryDate { get; private set; }
    public int Priority { get; private set; }
    public OrderType OrderType { get; private set; }
    public OrderStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public int? CustomWarningDays { get; private set; }
    public int? CustomCriticalDays { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order()
    {
    }

    public static Order Create(Guid tenantId, string orderNumber, DateTime deliveryDate,
        int priority, OrderType orderType, Guid createdByUserId, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new DomainException("INVALID_ORDER_NUMBER", "Order number is required.");
        if (deliveryDate.Date <= DateTime.UtcNow.Date)
            throw new DomainException("INVALID_DATE", "Delivery date must be in the future.");
        if (priority <= 0)
            throw new DomainException("INVALID_PRIORITY", "Priority must be positive.");

        var order = new Order
        {
            TenantId = tenantId,
            OrderNumber = orderNumber.Trim(),
            DeliveryDate = deliveryDate,
            Priority = priority,
            OrderType = orderType,
            Status = OrderStatus.Draft,
            Notes = notes?.Trim()
        };
        order.SetCreated(createdByUserId);
        return order;
    }

    public OrderItem AddItem(Guid productCategoryId, string productName, int quantity, string? notes = null)
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException("ORDER_NOT_DRAFT", "Can only add items to draft orders.");
        if (quantity <= 0)
            throw new DomainException("INVALID_QUANTITY", "Quantity must be positive.");

        var item = OrderItem.Create(TenantId, Id, productCategoryId, productName, quantity, notes);
        _items.Add(item);
        return item;
    }

    public void RemoveItem(Guid itemId)
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException("ORDER_NOT_DRAFT", "Can only remove items from draft orders.");

        var item = _items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new DomainException("ITEM_NOT_FOUND", "Order item not found.");
        _items.Remove(item);
    }

    public void Activate()
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException("INVALID_STATUS", "Only draft orders can be activated.");
        if (!_items.Any())
            throw new DomainException("NO_ITEMS", "Order must have at least one item.");

        Status = OrderStatus.Active;
    }

    public void Pause()
    {
        if (Status != OrderStatus.Active)
            throw new DomainException("INVALID_STATUS", "Only active orders can be paused.");
        Status = OrderStatus.Paused;
    }

    public void Resume()
    {
        if (Status != OrderStatus.Paused)
            throw new DomainException("INVALID_STATUS", "Only paused orders can be resumed.");
        Status = OrderStatus.Active;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
            throw new DomainException("INVALID_STATUS", "Cannot cancel completed or already cancelled orders.");
        Status = OrderStatus.Cancelled;
    }

    public void ChangePriority(int newPriority)
    {
        if (newPriority <= 0)
            throw new DomainException("INVALID_PRIORITY", "Priority must be positive.");
        if (HasProductionStarted())
            throw new DomainException("PRODUCTION_STARTED", "Cannot change priority after production started.");
        Priority = newPriority;
    }

    public bool HasProductionStarted()
    {
        return _items.Any(i => i.Processes.Any(p => p.Status != Enums.ProcessStatus.Pending));
    }

    public void SetCustomWarningDays(int? warningDays, int? criticalDays)
    {
        CustomWarningDays = warningDays;
        CustomCriticalDays = criticalDays;
    }

    public void MarkCompleted()
    {
        if (_items.All(i => i.IsCompleted()))
            Status = OrderStatus.Completed;
    }

    public void Update(string? notes, int? customWarningDays, int? customCriticalDays)
    {
        Notes = notes?.Trim();
        CustomWarningDays = customWarningDays;
        CustomCriticalDays = customCriticalDays;
    }
}
