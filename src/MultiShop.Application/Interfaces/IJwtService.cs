using MultiShop.Domain.Entities;

namespace MultiShop.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(AppUser user);
}