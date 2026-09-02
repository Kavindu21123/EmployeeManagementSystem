using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;

    // Dependency Injection: The interface is injected here!
    public EmployeeService(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        var employees = await _repository.GetAllAsync();
        
        // Mapping the Entity to the DTO
        return employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Salary = e.Salary,
            DepartmentName = e.Department?.Name ?? "No Department"
        });
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id);
        if (e == null) return null;

        return new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Salary = e.Salary,
            DepartmentName = e.Department?.Name ?? "No Department"
        };
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto)
    {
        // Map DTO to Domain Entity
        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Salary = dto.Salary,
            DepartmentId = dto.DepartmentId
        };

        await _repository.AddAsync(employee);

        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Salary = employee.Salary
        };
    }

    public async Task UpdateEmployeeAsync(UpdateEmployeeDto dto)
    {
        var existing = await _repository.GetByIdAsync(dto.Id);
        if (existing == null) throw new Exception("Employee not found");

        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Salary = dto.Salary;

        await _repository.UpdateAsync(existing);
    }

    public async Task DeleteEmployeeAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing != null)
        {
            await _repository.DeleteAsync(existing);
        }
    }
}