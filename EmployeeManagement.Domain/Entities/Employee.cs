namespace EmployeeManagement.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public decimal Salary { get; set; }

    // Foreign Key: Links this employee to a specific Department
    public int DepartmentId { get; set; }
    
    // Navigation property: Allows EF Core to easily load the actual Department object
    public Department? Department { get; set; }
}