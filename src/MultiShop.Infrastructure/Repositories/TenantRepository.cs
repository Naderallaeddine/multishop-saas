using Microsoft.EntityFrameworkCore;
using MultiShop.Application.Interfaces;
using MultiShop.Domain.Entities;
using MultiShop.Infrastructure.Persistence;

namespace MultiShop.Infrastructure.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly ApplicationDbContext _context;

    public TenantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tenant>> GetAllAsync() =>
        await _context.Tenants.Where(t => t.IsActive).ToListAsync();

    public async Task<Tenant?> GetByIdAsync(Guid id) =>
        await _context.Tenants.FindAsync(id);

    public async Task<Tenant?> GetBySlugAsync(string slug) =>
        await _context.Tenants.FirstOrDefaultAsync(t => t.Slug == slug);

    public async Task<Tenant> CreateAsync(Tenant tenant)
    {
        tenant.Id = Guid.NewGuid();
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();
        return tenant;
    }

    public async Task UpdateAsync(Tenant tenant)
    {
        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var tenant = await _context.Tenants.FindAsync(id);
        if (tenant != null)
        {
            tenant.IsActive = false; // soft delete
            await _context.SaveChangesAsync();
        }
    }
}