using AlGreenMES.Modules.Production.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Api.Requests;

public record UpdateOrderRequest(
    string? OrderNumber,
    DateTime? DeliveryDate,
    string? Notes,
    int? CustomWarningDays,
    int? CustomCriticalDays,
    List<UpdateOrderAddItemInput>? AddItems = null,
    List<Guid>? RemoveItemIds = null,
    List<UpdateOrderComplexityOverrideInput>? ComplexityOverrides = null,
    List<UpdateOrderAddSpecialRequestInput>? AddSpecialRequests = null,
    List<UpdateOrderRemoveSpecialRequestInput>? RemoveSpecialRequests = null);

public record UpdateOrderAddItemInput(Guid ProductCategoryId, string? ProductName, int Quantity, string? Notes);
public record UpdateOrderComplexityOverrideInput(Guid ItemId, Guid ProcessId, ComplexityType Complexity);
public record UpdateOrderAddSpecialRequestInput(Guid ItemId, Guid SpecialRequestTypeId);
public record UpdateOrderRemoveSpecialRequestInput(Guid ItemId, Guid SpecialRequestId);
