using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

// Implements the Application-layer contract — AppDbContext never leaks past this point
public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _dbContext;

    public AdminRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Admin?> GetByUsernameAsync(string username)
    {
        return await _dbContext.Admins
            .FirstOrDefaultAsync(a => a.Username == username);
    }
}

