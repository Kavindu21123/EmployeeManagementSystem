using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Set table name (optional, but good practice)
        builder.ToTable("Employees");

        // Make FirstName required and set a max length
        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        // Make LastName required and set a max length
        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(50);

        // Set column type for money to prevent rounding errors
        builder.Property(e => e.Salary)
            .HasColumnType("decimal(18,2)");

        // Configure the One-to-Many Relationship explicitly
        builder.HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict); // Prevents deleting a department if it has employees
    }
}