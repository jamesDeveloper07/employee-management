using Employee.Application.Commands;
using Employee.Domain.Repositories;
using MediatR;

namespace Employee.Application.Handlers;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, bool>
{
    private readonly IEmployeeRepository _employeeRepository;

    public DeleteEmployeeCommandHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (employee == null)
            return false;

        _employeeRepository.Delete(employee);
        await _employeeRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return true;
    }
}
