using Common.Domain;
using Employee.Domain.Aggregates;

namespace Employee.Domain.Repositories;

public interface IEmployeeRepository : IRepository<EmployeeAggregate>
{
    Task<EmployeeAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EmployeeAggregate?> GetByCPFAsync(string cpf, CancellationToken cancellationToken = default);
    Task<EmployeeAggregate?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeAggregate>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeAggregate>> GetActiveEmployeesAsync(CancellationToken cancellationToken = default);

    EmployeeAggregate Add(EmployeeAggregate employee);
    EmployeeAggregate Update(EmployeeAggregate employee);
    void Delete(EmployeeAggregate employee);
}
