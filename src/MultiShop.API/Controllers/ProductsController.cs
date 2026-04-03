using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Application.DTOs;
using MultiShop.Application.Interfaces;
using MultiShop.Domain.Entities;

namespace MultiShop.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repo;

    public ProductsController(IProductRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("tenant/{tenantId:guid}")]
    public async Task<IActionResult> GetByTenant(Guid tenantId)
    {
        var products = await _repo.GetByTenantAsync(tenantId);
        var result = products.Select(p => new ProductDto(
            p.Id, p.TenantId, p.Name, p.Description,
            p.Price, p.Stock, p.IsActive));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(new ProductDto(
            product.Id, product.TenantId, product.Name,
            product.Description, product.Price,
            product.Stock, product.IsActive));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var product = new Product
        {
            TenantId = dto.TenantId,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock
        };

        var created = await _repo.CreateAsync(product);
        return CreatedAtAction(nameof(GetById),
            new { id = created.Id },
            new ProductDto(created.Id, created.TenantId, created.Name,
                created.Description, created.Price,
                created.Stock, created.IsActive));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return NotFound();

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.IsActive = dto.IsActive;

        await _repo.UpdateAsync(product);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _repo.DeleteAsync(id);
        return NoContent();
    }
}