namespace MultiShop.Application.Interfaces;

public interface IOrderNotificationService
{
    Task NotifyNewOrder(string tenantId, object orderSummary);
    Task NotifyOrderStatusChanged(string tenantId, Guid orderId, string newStatus);
}