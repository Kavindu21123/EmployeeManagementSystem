using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

// Notice we are implementing the interface from the Application layer!
public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _dbContext;

    public EmployeeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        // .Include(e => e.Department) tells EF Core to do a SQL JOIN 
        // so we get the Department data along with the Employee
        return await _dbContext.Employees
            .Include(e => e.Department)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _dbContext.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task AddAsync(Employee employee)
    {
        await _dbContext.Employees.AddAsync(employee);
        await _dbContext.SaveChangesAsync(); // Actually executes the SQL INSERT
    }

    public async Task UpdateAsync(Employee employee)
    {
        _dbContext.Employees.Update(employee);
        await _dbContext.SaveChangesAsync(); // Executes the SQL UPDATE
    }

    public async Task DeleteAsync(Employee employee)
    {
        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync(); // Executes the SQL DELETE
    }
}