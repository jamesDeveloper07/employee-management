using Employee.Application.Commands;
using Employee.Domain.Repositories;
using MediatR;

namespace Employee.Application.Handlers;

public class ActivateEmployeeCommandHandler : IRequestHandler<ActivateEmployeeCommand, bool>
{
    private readonly IEmployeeRepository _employeeRepository;

    public ActivateEmployeeCommandHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<bool> Handle(ActivateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (employee == null)
            return false;

        employee.Activate();

        _employeeRepository.Update(employee);
        await _employeeRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return true;
    }
}
