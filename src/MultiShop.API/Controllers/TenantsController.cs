
using Microsoft.AspNetCore.Mvc;
using MultiShop.Application.DTOs;
using MultiShop.Application.Interfaces;
using MultiShop.Domain.Entities;

namespace MultiShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ITenantRepository _repo;

    public TenantsController(ITenantRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tenants = await _repo.GetAllAsync();
        var result = tenants.Select(t => new TenantDto(
            t.Id, t.Name, t.Slug, t.OwnerEmail, t.IsActive, t.CreatedAt));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var tenant = await _repo.GetByIdAsync(id);
        if (tenant == null) return NotFound();
        return Ok(new TenantDto(
            tenant.Id, tenant.Name, tenant.Slug,
            tenant.OwnerEmail, tenant.IsActive, tenant.CreatedAt));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantDto dto)
    {
        var tenant = new Tenant
        {
            Name = dto.Name,
            Slug = dto.Slug.ToLower().Replace(" ", "-"),
            OwnerEmail = dto.OwnerEmail
        };

        var created = await _repo.CreateAsync(tenant);
        return CreatedAtAction(nameof(GetById),
            new { id = created.Id },
            new TenantDto(created.Id, created.Name, created.Slug,
                created.OwnerEmail, created.IsActive, created.CreatedAt));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _repo.DeleteAsync(id);
        return NoContent();
    }
}