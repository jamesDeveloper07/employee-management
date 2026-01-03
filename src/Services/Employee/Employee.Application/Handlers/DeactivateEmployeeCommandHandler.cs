using Employee.Application.Commands;
using Employee.Domain.Repositories;
using MediatR;

namespace Employee.Application.Handlers;

public class DeactivateEmployeeCommandHandler : IRequestHandler<DeactivateEmployeeCommand, bool>
{
    private readonly IEmployeeRepository _employeeRepository;

    public DeactivateEmployeeCommandHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<bool> Handle(DeactivateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (employee == null)
            return false;

        employee.Deactivate();

        _employeeRepository.Update(employee);
        await _employeeRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return true;
    }
}
