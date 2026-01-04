using Microsoft.EntityFrameworkCore;
using Common.Domain;
using Employee.Domain.Entities;
using Employee.Domain.Repositories;
using Employee.Infrastructure.Persistence;

namespace Employee.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly EmployeeDbContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public DepartmentRepository(EmployeeDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(d => d.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Department>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public Department Add(Department department)
    {
        return _context.Departments.Add(department).Entity;
    }

    public Department Update(Department department)
    {
        return _context.Departments.Update(department).Entity;
    }

    public void Delete(Department department)
    {
        _context.Departments.Remove(department);
    }
}
