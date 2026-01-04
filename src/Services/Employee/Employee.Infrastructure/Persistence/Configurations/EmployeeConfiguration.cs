using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Employee.Domain.Aggregates;
using Employee.Domain.ValueObjects;

namespace Employee.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<EmployeeAggregate>
{
    public void Configure(EntityTypeBuilder<EmployeeAggregate> builder)
    {
        // Table name
        builder.ToTable("employees");

        // Primary key
        builder.HasKey(e => e.Id);

        // Properties
        builder.Property(e => e.Id)
            .ValueGeneratedNever(); // We generate Guid in domain

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.BirthDate)
            .IsRequired();

        builder.Property(e => e.HireDate)
            .IsRequired();

        builder.Property(e => e.Salary)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.Position)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);

        // Value Objects - CPF
        builder.OwnsOne(e => e.CPF, cpf =>
        {
            cpf.Property(c => c.Value)
                .HasColumnName("cpf")
                .IsRequired()
                .HasMaxLength(11);

            cpf.HasIndex(c => c.Value)
                .IsUnique()
                .HasDatabaseName("ix_employees_cpf");
        });

        // Value Objects - Email
        builder.OwnsOne(e => e.Email, email =>
        {
            email.Property(em => em.Value)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(255);

            email.HasIndex(em => em.Value)
                .IsUnique()
                .HasDatabaseName("ix_employees_email");
        });

        // Value Objects - PhoneNumber
        builder.OwnsOne(e => e.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("phone_number")
                .IsRequired()
                .HasMaxLength(11);
        });

        // Value Objects - Address (Complex)
        builder.OwnsOne(e => e.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("address_street")
                .IsRequired()
                .HasMaxLength(200);

            address.Property(a => a.Number)
                .HasColumnName("address_number")
                .IsRequired()
                .HasMaxLength(20);

            address.Property(a => a.Complement)
                .HasColumnName("address_complement")
                .IsRequired(false)
                .HasMaxLength(100);

            address.Property(a => a.Neighborhood)
                .HasColumnName("address_neighborhood")
                .IsRequired()
                .HasMaxLength(100);

            address.Property(a => a.City)
                .HasColumnName("address_city")
                .IsRequired()
                .HasMaxLength(100);

            address.Property(a => a.State)
                .HasColumnName("address_state")
                .IsRequired()
                .HasMaxLength(2);

            address.Property(a => a.ZipCode)
                .HasColumnName("address_zip_code")
                .IsRequired()
                .HasMaxLength(8);
        });

        // Foreign Key - Department
        builder.Property(e => e.DepartmentId)
            .IsRequired();

        builder.HasOne(e => e.Department)
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes (excluding owned entities - they have their own indexes defined above)
        builder.HasIndex(e => e.DepartmentId);

        builder.HasIndex(e => e.IsActive);

        builder.HasIndex(e => e.HireDate);

        // Ignore Domain Events (not persisted)
        builder.Ignore(e => e.DomainEvents);
    }
}
