using Employee.Domain.Entities;

namespace Employee.Domain.Repositories;

public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Department>> GetActiveAsync(CancellationToken cancellationToken = default);

    Department Add(Department department);
    Department Update(Department department);
    void Delete(Department department);
}
