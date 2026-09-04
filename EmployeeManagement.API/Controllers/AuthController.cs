using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    // Inject the config (for the secret key) and the database
    public AuthController(IConfiguration configuration, AppDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        // 1. Search the database for this specific username
        var admin = await _context.Admins.FirstOrDefaultAsync(u => u.Username == request.Username);

        // 2. If the user doesn't exist, kick them out immediately
        if (admin == null)
        {
            return Unauthorized("Invalid credentials.");
        }

        // 3. Verify the password using BCrypt
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash);

        if (!isPasswordValid)
        {
            return Unauthorized("Invalid credentials.");
        }

        // 4. Build the Claims (The data stamped onto the card)
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, admin.Username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        // 5. Retrieve the Secret Key from appsettings.json
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 6. Print the digital key card
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1), // Card expires in 1 hour
            signingCredentials: creds
        );

        // 7. Hand the card to the user as a text string
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { Token = jwt });
    }
}