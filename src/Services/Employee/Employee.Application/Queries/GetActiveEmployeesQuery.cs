using Employee.Application.DTOs;
using MediatR;

namespace Employee.Application.Queries;

public class GetActiveEmployeesQuery : IRequest<IEnumerable<EmployeeDto>>
{
}
