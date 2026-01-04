using Microsoft.EntityFrameworkCore;
using Employee.Domain.Aggregates;
using Employee.Domain.Repositories;
using Employee.Infrastructure.Persistence;
using Common.Domain;

namespace Employee.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly EmployeeDbContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public EmployeeRepository(EmployeeDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<EmployeeAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<EmployeeAggregate?> GetByCPFAsync(string cpf, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.CPF.Value == cpf, cancellationToken);
    }

    public async Task<EmployeeAggregate?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Email.Value == email, cancellationToken);
    }

    public async Task<IEnumerable<EmployeeAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<EmployeeAggregate>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.DepartmentId == departmentId)
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<EmployeeAggregate>> GetActiveEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.IsActive)
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .ToListAsync(cancellationToken);
    }

    public EmployeeAggregate Add(EmployeeAggregate employee)
    {
        return _context.Employees.Add(employee).Entity;
    }

    public EmployeeAggregate Update(EmployeeAggregate employee)
    {
        return _context.Employees.Update(employee).Entity;
    }

    public void Delete(EmployeeAggregate employee)
    {
        _context.Employees.Remove(employee);
    }
}
