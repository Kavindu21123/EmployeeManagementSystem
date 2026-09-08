using EmployeeManagement.Domain;

namespace EmployeeManagement.Application.Interfaces;

public interface IAdminRepository
{
    Task<Admin?> GetByUsernameAsync(string username);
}

