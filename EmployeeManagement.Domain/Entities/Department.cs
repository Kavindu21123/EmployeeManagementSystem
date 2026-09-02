namespace EmployeeManagement.Domain.Entities;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation property: A department can have many employees.
    // We initialize it to an empty list to avoid Null Reference errors.
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}