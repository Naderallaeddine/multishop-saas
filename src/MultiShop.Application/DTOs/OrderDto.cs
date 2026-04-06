using MultiShop.Domain.Enums;

namespace MultiShop.Application.DTOs;

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);

public record OrderDto(
    Guid Id,
    Guid TenantId,
    string CustomerId,
    DateTime OrderDate,
    OrderStatus Status,
    string StatusName,
    decimal TotalAmount,
    List<OrderItemDto> Items
);

public record CreateOrderDto(
    Guid TenantId,
    List<CreateOrderItemDto> Items
);

public record CreateOrderItemDto(
    Guid ProductId,
    int Quantity
);