using MultiShop.Domain.Entities;

namespace MultiShop.Application.Interfaces;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetByTenantAsync(Guid tenantId);
    Task<IEnumerable<Order>> GetByCustomerAsync(string customerId);
    Task<Order?> GetByIdAsync(Guid id);
    Task<Order> CreateAsync(Order order);
    Task UpdateStatusAsync(Guid id, Domain.Enums.OrderStatus status);
}