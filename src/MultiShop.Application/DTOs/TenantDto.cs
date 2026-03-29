namespace MultiShop.Application.DTOs;

public record TenantDto(
    Guid Id,
    string Name,
    string Slug,
    string OwnerEmail,
    bool IsActive,
    DateTime CreatedAt
);

public record CreateTenantDto(
    string Name,
    string Slug,
    string OwnerEmail
);