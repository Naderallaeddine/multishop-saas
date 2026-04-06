using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Application.DTOs;
using MultiShop.Application.Interfaces;
using MultiShop.Domain.Entities;
using MultiShop.Domain.Enums;

namespace MultiShop.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepo;
    private readonly IProductRepository _productRepo;
    private readonly IOrderNotificationService _notifications;

    public OrdersController(
        IOrderRepository orderRepo,
        IProductRepository productRepo,
        IOrderNotificationService notifications)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _notifications = notifications;
    }

    [HttpGet("tenant/{tenantId:guid}")]
    public async Task<IActionResult> GetByTenant(Guid tenantId)
    {
        var orders = await _orderRepo.GetByTenantAsync(tenantId);
        return Ok(orders.Select(MapToDto));
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrders()
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var orders = await _orderRepo.GetByCustomerAsync(customerId);
        return Ok(orders.Select(MapToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _orderRepo.GetByIdAsync(id);
        if (order == null) return NotFound();
        return Ok(MapToDto(order));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var items = new List<OrderItem>();
        decimal total = 0;

        foreach (var itemDto in dto.Items)
        {
            var product = await _productRepo.GetByIdAsync(itemDto.ProductId);
            if (product == null)
                return BadRequest($"Product {itemDto.ProductId} not found");

            if (product.Stock < itemDto.Quantity)
                return BadRequest($"Insufficient stock for {product.Name}");

            items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price
            });

            product.Stock -= itemDto.Quantity;
            await _productRepo.UpdateAsync(product);
            total += product.Price * itemDto.Quantity;
        }

        var order = new Order
        {
            TenantId = dto.TenantId,
            CustomerId = customerId,
            Items = items,
            TotalAmount = total
        };

        var created = await _orderRepo.CreateAsync(order);
        var result = await _orderRepo.GetByIdAsync(created.Id);

        // Send real-time notification to store owner
        await _notifications.NotifyNewOrder(dto.TenantId.ToString(), new
        {
            orderId = created.Id,
            totalAmount = total,
            itemCount = items.Count,
            placedAt = DateTime.UtcNow
        });

        return CreatedAtAction(nameof(GetById),
            new { id = created.Id }, MapToDto(result!));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id, [FromBody] OrderStatus status)
    {
        var order = await _orderRepo.GetByIdAsync(id);
        if (order == null) return NotFound();

        await _orderRepo.UpdateStatusAsync(id, status);

        // Notify status change
        await _notifications.NotifyOrderStatusChanged(
            order.TenantId.ToString(), id, status.ToString());

        return NoContent();
    }

    private static OrderDto MapToDto(Order o) => new(
        o.Id, o.TenantId, o.CustomerId,
        o.OrderDate, o.Status, o.Status.ToString(),
        o.TotalAmount,
        o.Items.Select(i => new OrderItemDto(
            i.ProductId,
            i.Product?.Name ?? "",
            i.Quantity,
            i.UnitPrice,
            i.TotalPrice
        )).ToList()
    );
}