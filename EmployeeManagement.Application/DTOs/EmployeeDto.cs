namespace EmployeeManagement.Application.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
}