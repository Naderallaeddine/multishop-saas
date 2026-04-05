using Microsoft.AspNetCore.SignalR;
using MultiShop.API.Hubs;
using MultiShop.Application.Interfaces;

namespace MultiShop.API.Services;

public class OrderNotificationService : IOrderNotificationService
{
    private readonly IHubContext<OrderHub> _hubContext;

    public OrderNotificationService(IHubContext<OrderHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyNewOrder(string tenantId, object orderSummary)
    {
        await _hubContext.Clients
            .Group($"tenant-{tenantId}")
            .SendAsync("NewOrder", orderSummary);
    }

    public async Task NotifyOrderStatusChanged(
        string tenantId, Guid orderId, string newStatus)
    {
        await _hubContext.Clients
            .Group($"tenant-{tenantId}")
            .SendAsync("OrderStatusChanged", new { orderId, newStatus });
    }
}