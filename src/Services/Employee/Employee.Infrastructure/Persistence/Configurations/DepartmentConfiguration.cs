using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Employee.Domain.Entities;

namespace Employee.Infrastructure.Persistence.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        // Table name
        builder.ToTable("Departments");

        // Primary key
        builder.HasKey(d => d.Id);

        // Properties
        builder.Property(d => d.Id)
            .ValueGeneratedNever(); // We generate Guid in domain

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(d => d.Name)
            .IsUnique();

        builder.HasIndex(d => d.IsActive);
    }
}
