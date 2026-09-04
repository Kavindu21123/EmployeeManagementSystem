using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    // Dependency Injection connects the Service here
    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // GET: api/employee
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        return Ok(employees); // Returns HTTP 200 with the data
    }

    // GET: api/employee/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null) return NotFound(); // Returns HTTP 404

        return Ok(employee);
    }

    // POST: api/employee
    [HttpPost]
    public async Task<IActionResult> Create(CreateEmployeeDto dto)
    {
        var createdEmployee = await _employeeService.CreateEmployeeAsync(dto);
        
        // Returns HTTP 201 (Created) and points to the GET endpoint for the new resource
        return CreatedAtAction(nameof(GetById), new { id = createdEmployee.Id }, createdEmployee);
    }

    // PUT: api/employee/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateEmployeeDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("ID mismatch");
        }

        // Call the service without a try-catch. 
        // If validation fails, or if the employee isn't found, the Global Exception Handler will catch it automatically.
        await _employeeService.UpdateEmployeeAsync(dto);

        return NoContent();
    }


    // DELETE: api/employee/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _employeeService.DeleteEmployeeAsync(id);
        return NoContent(); // Returns HTTP 204 (Successfully deleted)
    }

}