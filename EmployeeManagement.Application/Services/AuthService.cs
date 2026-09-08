using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IAdminRepository adminRepository, IConfiguration configuration)
    {
        _adminRepository = adminRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        // 1. Look up the admin by username via the repository abstraction
        var admin = await _adminRepository.GetByUsernameAsync(dto.Username);

        // 2. If user doesn't exist, return null — controller will handle the 401
        if (admin == null) return null;

        // 3. Verify the plain-text password against the stored hash
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, admin.PasswordHash);
        if (!isPasswordValid) return null;

        // 4. Build the claims to be stamped into the token
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, admin.Username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        // 5. Retrieve the secret key from configuration
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 6. Build the JWT token
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        // 7. Serialize the token to a string and return it
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthResponseDto { Token = jwt };
    }
}

