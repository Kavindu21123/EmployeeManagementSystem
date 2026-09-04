using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using FluentValidation;

namespace EmployeeManagement.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IValidator<CreateEmployeeDto> _validator; // 2. Add the bouncer
    private readonly IValidator<UpdateEmployeeDto> _updateValidator; // 1. Add the new Update bouncer

    // Dependency Injection: The interface is injected here!
    public EmployeeService(IEmployeeRepository repository, 
                           IValidator<CreateEmployeeDto> validator,
                           IValidator<UpdateEmployeeDto> updateValidator)
    {
        _repository = repository;
        _validator = validator;
        _updateValidator = updateValidator;
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

        // 4. Check the rules before doing anything else
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            // If it fails, throw an error immediately 
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException($"Validation failed: {errors}");
        }

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
        // 1. Check the rules before doing anything else!

        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException($"Validation failed: {errors}");
        }


        var existing = await _repository.GetByIdAsync(dto.Id);
        if (existing == null) throw new KeyNotFoundException($"Employee with ID {dto.Id} was not found.");

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