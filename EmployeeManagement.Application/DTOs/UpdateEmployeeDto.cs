namespace EmployeeManagement.Application.DTOs;

public class UpdateEmployeeDto
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public decimal Salary { get; set; }
}