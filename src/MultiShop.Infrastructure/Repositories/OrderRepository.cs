using Microsoft.EntityFrameworkCore;
using MultiShop.Application.Interfaces;
using MultiShop.Domain.Entities;
using MultiShop.Domain.Enums;
using MultiShop.Infrastructure.Persistence;

namespace MultiShop.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetByTenantAsync(Guid tenantId) =>
        await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.TenantId == tenantId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

    public async Task<IEnumerable<Order>> GetByCustomerAsync(string customerId) =>
        await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

    public async Task<Order?> GetByIdAsync(Guid id) =>
        await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<Order> CreateAsync(Order order)
    {
        order.Id = Guid.NewGuid();
        foreach (var item in order.Items)
            item.Id = Guid.NewGuid();

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task UpdateStatusAsync(Guid id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            order.Status = status;
            await _context.SaveChangesAsync();
        }
    }
}