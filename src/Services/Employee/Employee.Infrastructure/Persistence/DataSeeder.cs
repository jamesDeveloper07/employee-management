using Employee.Domain.Entities;
using Employee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Employee.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<EmployeeDbContext>>();

        try
        {
            // Ensure database is created
            await context.Database.MigrateAsync();

            // Seed Departments if none exist
            if (!await context.Departments.AnyAsync())
            {
                logger.LogInformation("Seeding initial departments...");

                var departmentsData = new[]
                {
                    ("Tecnologia da Informação", "Departamento responsável pela infraestrutura tecnológica e desenvolvimento de sistemas"),
                    ("Recursos Humanos", "Departamento responsável pela gestão de pessoas, recrutamento e desenvolvimento"),
                    ("Financeiro", "Departamento responsável pela gestão financeira e contábil da empresa"),
                    ("Comercial", "Departamento responsável pelas vendas e relacionamento com clientes"),
                    ("Marketing", "Departamento responsável pela comunicação, branding e marketing digital"),
                    ("Operações", "Departamento responsável pela gestão operacional e logística"),
                    ("Jurídico", "Departamento responsável por questões legais e compliance"),
                    ("Qualidade", "Departamento responsável pelo controle de qualidade e processos")
                };

                var departments = departmentsData.Select(d => Department.Create(d.Item1, d.Item2)).ToList();
                await context.Departments.AddRangeAsync(departments);
                await context.SaveChangesAsync();

                logger.LogInformation("Initial departments seeded successfully. {Count} departments created.", departmentsData.Length);
            }
            else
            {
                logger.LogInformation("Departments already exist. Skipping seed.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}
