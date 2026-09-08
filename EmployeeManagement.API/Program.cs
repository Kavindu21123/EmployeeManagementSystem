using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Application.Validators;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Enable Controllers
builder.Services.AddControllers();

// 2. Register Entity Framework with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Register Dependency Injection (The Matchmaker)
// AddScoped means a new instance is created once per HTTP request
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IValidator<CreateEmployeeDto>, CreateEmployeeDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateEmployeeDto>, UpdateEmployeeDtoValidator>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


builder.Services.AddAuthentication(options =>
{
    // Tells the API to strictly use JWTs for authentication, not cookies
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        // This converts your secret string into a cryptographic key
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!))
    };
});

// Adds authorization so we can use roles (like "Admin") later
builder.Services.AddAuthorization();



var app = builder.Build();

app.UseExceptionHandler();

// --- ADD THESE TWO LINES EXACTLY HERE ---
app.UseAuthentication(); // Checkpoint 1: Who are you? (Validates the JWT)
app.UseAuthorization();  // Checkpoint 2: What are you allowed to do?

// 4. Map the endpoints
app.MapControllers();

// --- TEMPORARY CODE TO CREATE AN ADMIN ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EmployeeManagement.Infrastructure.Data.AppDbContext>();

    // Check if an admin already exists
    if (!context.Admins.Any())
    {
        // Blend the password!
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword("SuperSecretPassword!");

        var firstAdmin = new EmployeeManagement.Domain.Admin
        {
            Username = "boss",
            PasswordHash = hashedPassword
        };

        context.Admins.Add(firstAdmin);
        context.SaveChanges();
    }
}
// -----------------------------------------

app.Run();

