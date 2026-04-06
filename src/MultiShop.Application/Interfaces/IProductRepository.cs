using MultiShop.Domain.Entities;

namespace MultiShop.Application.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetByTenantAsync(Guid tenantId);
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product> CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
}