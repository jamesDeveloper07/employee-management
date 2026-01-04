using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Employee.Infrastructure.Persistence;

namespace Employee.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EmployeeDbContext>
{
    public EmployeeDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: false)
            .Build();

        var builder = new DbContextOptionsBuilder<EmployeeDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.MigrationsAssembly(typeof(EmployeeDbContext).Assembly.FullName);
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        });

        return new EmployeeDbContext(builder.Options);
    }
}
