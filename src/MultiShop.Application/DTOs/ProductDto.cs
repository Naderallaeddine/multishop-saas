namespace MultiShop.Application.DTOs;

public record ProductDto(
    Guid Id,
    Guid TenantId,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    bool IsActive
);

public record CreateProductDto(
    Guid TenantId,
    string Name,
    string Description,
    decimal Price,
    int Stock
);

public record UpdateProductDto(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    bool IsActive
);