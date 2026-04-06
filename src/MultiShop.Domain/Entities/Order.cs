using MultiShop.Domain.Enums;

namespace MultiShop.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public List<OrderItem> Items { get; set; } = new();

    public Tenant Tenant { get; set; } = null!;
    public AppUser Customer { get; set; } = null!;
}