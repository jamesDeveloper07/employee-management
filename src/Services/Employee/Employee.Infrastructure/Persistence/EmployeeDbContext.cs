using Microsoft.EntityFrameworkCore;
using Employee.Domain.Aggregates;
using Employee.Domain.Entities;
using Common.Domain;

namespace Employee.Infrastructure.Persistence;

public class EmployeeDbContext : DbContext, IUnitOfWork
{
    public DbSet<EmployeeAggregate> Employees { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;

    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmployeeDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events before saving
        var domainEvents = GetDomainEvents();

        var result = await base.SaveChangesAsync(cancellationToken);

        // Domain events will be dispatched via MediatR after saving
        // This will be implemented in the API layer or via a domain event dispatcher

        return result;
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Dispatch domain events before saving
            var domainEvents = GetDomainEvents();

            // Debug: Check if there are changes to save
            var changesCount = ChangeTracker.Entries().Count(e => e.State == EntityState.Added ||
                                                                   e.State == EntityState.Modified ||
                                                                   e.State == EntityState.Deleted);

            Console.WriteLine($"[SaveEntitiesAsync] Changes to save: {changesCount}");

            // Save changes
            var result = await base.SaveChangesAsync(cancellationToken);

            Console.WriteLine($"[SaveEntitiesAsync] Saved {result} entities");

            // Domain events will be dispatched via MediatR after saving
            // This will be implemented in the API layer or via a domain event dispatcher

            return result > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SaveEntitiesAsync] ERROR: {ex.Message}");
            throw;
        }
    }

    private List<IDomainEvent> GetDomainEvents()
    {
        var domainEvents = ChangeTracker.Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .SelectMany(e =>
            {
                var events = e.DomainEvents.ToList();
                e.ClearDomainEvents();
                return events;
            })
            .ToList();

        return domainEvents;
    }
}
