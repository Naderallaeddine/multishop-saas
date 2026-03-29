using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Application.DTOs;
using MultiShop.Application.Interfaces;
using MultiShop.Domain.Entities;

namespace MultiShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtService _jwtService;

    public AuthController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return BadRequest(new { message = "Email already in use" });

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Role = "Customer"
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var token = _jwtService.GenerateToken(user);

        return Ok(new AuthResponseDto(
            token,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.Role,
            DateTime.UtcNow.AddMinutes(60)
        ));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Unauthorized(new { message = "Invalid email or password" });

        var result = await _signInManager.CheckPasswordSignInAsync(
            user, dto.Password, false);

        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid email or password" });

        var token = _jwtService.GenerateToken(user);

        return Ok(new AuthResponseDto(
            token,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.Role,
            DateTime.UtcNow.AddMinutes(60)
        ));
    }
}