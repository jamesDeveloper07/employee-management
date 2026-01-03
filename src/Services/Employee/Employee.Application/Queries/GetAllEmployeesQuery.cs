using Employee.Application.DTOs;
using MediatR;

namespace Employee.Application.Queries;

public class GetAllEmployeesQuery : IRequest<IEnumerable<EmployeeDto>>
{
}
